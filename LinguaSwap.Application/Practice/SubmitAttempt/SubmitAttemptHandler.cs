using LinguaSwap.Application.Practice.Common;
using LinguaSwap.Domain.Practice;
using LinguaSwap.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Practice.SubmitAttempt
{
    public sealed class SubmitAttemptHandler
    {
        private readonly InMemoryPracticeSessionStore _store;
        private readonly LinguaSwapDbContext _db;

        public SubmitAttemptHandler(InMemoryPracticeSessionStore store, LinguaSwapDbContext db)
        {
            _store = store;
            _db = db;
        }

        public SubmitAttemptResult Handle(SubmitAttemptCommand command)
        {
            var data = _store.Get(command.SessionId)
                ?? throw new InvalidOperationException("Session not found");

            if (data.Session.CurrentWordId is null)
                throw new InvalidOperationException("No current word. Call /next first.");

            if (command.WordId != data.Session.CurrentWordId.Value)
                throw new InvalidOperationException("You can only submit an attempt for the current word.");

            if (!data.State.ActiveVocabItemIds.Contains(command.WordId))
                throw new InvalidOperationException("Word not in active set");

            var source = data.Session.SourceLanguage;
            var target = data.Session.TargetLanguage;

            // Según dirección, intercambiamos idiomas
            if (data.Session.Direction == PracticeDirection.TargetToNative)
            {
                var temp = source;
                source = target;
                target = temp;
            }

            // Todas las traducciones válidas en idioma target
            var validAnswers = _db.VocabTerms
                .Where(t => t.VocabItemId == command.WordId && t.LanguageCode == target)
                .Select(t => t.Text)
                .ToList();

            if (!validAnswers.Any())
                throw new InvalidOperationException("No translations found for this item");

            static string Normalize(string s) => s.Trim().ToLowerInvariant();

            var normalizedUser = Normalize(command.UserAnswer);

            var isCorrect = validAnswers.Any(a => Normalize(a) == normalizedUser);

            // Para devolver algo en la respuesta
            var correctAnswer = validAnswers.First();

            // Stats: aseguramos que existe
            data.State.StatsByWordId.TryAdd(command.WordId, new WordPracticeStats());
            if (isCorrect) data.State.StatsByWordId[command.WordId].CorrectCount++;

            var attempt = new Attempt
            {
                SessionId = command.SessionId,
                WordId = command.WordId,
                UserAnswer = command.UserAnswer,
                IsCorrect = isCorrect
            };

            data.Attempts.Add(attempt);
            _db.Attempts.Add(attempt);

            if (data.Session.UserId is Guid userId)
            {
                var stats = _db.UserVocabStats.SingleOrDefault(s =>
                    s.UserId == userId &&
                    s.VocabItemId == command.WordId &&
                    s.SourceLanguage == data.Session.SourceLanguage &&
                    s.TargetLanguage == data.Session.TargetLanguage);

                if (stats is null)
                {
                    stats = new UserVocabStats
                    {
                        UserId = userId,
                        VocabItemId = command.WordId,
                        SourceLanguage = data.Session.SourceLanguage,
                        TargetLanguage = data.Session.TargetLanguage
                    };

                    _db.UserVocabStats.Add(stats);
                }

                if (isCorrect)
                    stats.CorrectCount++;
                else
                    stats.WrongCount++;

                stats.LastPracticedAt = DateTime.UtcNow;
            }

            data.Session.CurrentWordId = null;

            _db.SaveChanges();

            // ✅ Aquí metemos el crecimiento del set si ya toca
            TryGrowActiveSetIfNeeded(data);

            return new SubmitAttemptResult(isCorrect, correctAnswer);
        }

        private void TryGrowActiveSetIfNeeded(InMemoryPracticeSessionStore.SessionData data)
        {
            // Asegura stats para todas
            foreach (var id in data.State.ActiveVocabItemIds)
                data.State.StatsByWordId.TryAdd(id, new WordPracticeStats());

            var stats = data.State.StatsByWordId;

            var total = data.State.ActiveVocabItemIds.Count;
            if (total == 0) return;

            var learnedCount = data.State.ActiveVocabItemIds.Count(id => stats[id].IsLearned);
            var learnedRatio = (double)learnedCount / total;

            if (learnedRatio < 0.8) return;

            // Crece un 50% (ej: 20 -> +10)
            var growBy = Math.Max(1, (int)Math.Ceiling(total * 0.5));

            var source = data.Session.SourceLanguage;
            var target = data.Session.TargetLanguage;

            var query = _db.VocabItems.AsQueryable();

            // Filtro por biblioteca si existe
            if (data.Session.LibraryId is Guid libraryId)
                query = query.Where(i => i.LibraryId == libraryId);

            // Solo items que tengan ambos idiomas
            var allCandidateIds = query
                .Where(i =>
                    _db.VocabTerms.Any(t => t.VocabItemId == i.Id && t.LanguageCode == source) &&
                    _db.VocabTerms.Any(t => t.VocabItemId == i.Id && t.LanguageCode == target)
                )
                .Select(i => i.Id)
                .ToList();

            // Excluir los ya activos
            var candidates = allCandidateIds
                .Where(id => !data.State.ActiveVocabItemIds.Contains(id))
                .Take(growBy)
                .ToList();

            if (candidates.Count == 0) return;

            data.State.ActiveVocabItemIds.AddRange(candidates);

            // Nueva versión de crecimiento: habilita “review una vez” para aprendidas
            data.State.GrowthVersion++;

            // Muy importante: reiniciar iteración para aplicar prioridad de nuevas
            data.State.CurrentIterationQueue.Clear();

            // Inicializa stats nuevas
            foreach (var id in candidates)
                data.State.StatsByWordId.TryAdd(id, new WordPracticeStats());
        }
    }
}
