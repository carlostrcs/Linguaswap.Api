using LinguaSwap.Application.Practice.Common;
using LinguaSwap.Domain.Practice;
using LinguaSwap.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Practice.GetNextPracticeWord
{
    public sealed class GetNextPracticeWordHandler
    {
        private readonly InMemoryPracticeSessionStore _store;
        private readonly LinguaSwapDbContext _db;

        public GetNextPracticeWordHandler(InMemoryPracticeSessionStore store, LinguaSwapDbContext db)
        {
            _store = store;
            _db = db;
        }

        public GetNextPracticeWordResult Handle(GetNextPracticeWordQuery query)
        {
            _store.EnsureLoaded(query.SessionId, _db);
            var data = _store.Get(query.SessionId)
                ?? throw new InvalidOperationException("Session not found");
            var sourceLanguage = data.Session.SourceLanguage;
            var targetLanguage = data.Session.TargetLanguage;

            if (data.State.ActiveVocabItemIds.Count == 0)
            {
                // Query base: items que tengan term en source y en target
                var queryBase = _db.VocabItems.AsQueryable();

                // Filtro opcional por biblioteca
                if (data.Session.LibraryId is Guid libraryId)
                    queryBase = queryBase.Where(i => i.LibraryId == libraryId);

                // Items que tienen ambos idiomas (existe term source y term target)
                var candidateIds = queryBase
                    .Where(i =>
                        _db.VocabTerms.Any(t => t.VocabItemId == i.Id && t.LanguageCode == sourceLanguage) &&
                        _db.VocabTerms.Any(t => t.VocabItemId == i.Id && t.LanguageCode == targetLanguage)
                    )
                    .Select(i => i.Id)
                    .ToList();

                var initialSetSize = Math.Min(20, candidateIds.Count);
                data.State.ActiveVocabItemIds.AddRange(candidateIds.Take(initialSetSize));

                // Inicializa stats
                foreach (var id in data.State.ActiveVocabItemIds)
                    data.State.StatsByWordId.TryAdd(id, new WordPracticeStats());
            }

            if (data.State.CurrentIterationQueue.Count == 0)
            {
                // Asegura que todas las palabras tienen stats
                foreach (var id in data.State.ActiveVocabItemIds)
                    data.State.StatsByWordId.TryAdd(id, new WordPracticeStats());

                var stats = data.State.StatsByWordId;

                // Grupos según tu prioridad:
                // 1) Nuevas: nunca mostradas
                var newWords = data.State.ActiveVocabItemIds
                    .Where(id => stats[id].TimesShown == 0)
                    .ToList();

                // 2) En aprendizaje: NO aprendidas
                var learningWords = data.State.ActiveVocabItemIds
                    .Where(id => stats[id].TimesShown > 0 && !stats[id].IsLearned)
                    .ToList();

                // 3) Aprendidas: solo 1 vez tras cada crecimiento
                var learnedReview = data.State.ActiveVocabItemIds
                    .Where(id => stats[id].IsLearned && stats[id].LastReviewGrowthVersion < data.State.GrowthVersion)
                    .ToList();

                var rng = new Random();

                // Shuffle dentro de cada grupo (pero mantiene prioridad por grupo)
                newWords = newWords.OrderBy(_ => rng.Next()).ToList();
                learningWords = learningWords.OrderBy(_ => rng.Next()).ToList();
                learnedReview = learnedReview.OrderBy(_ => rng.Next()).ToList();

                foreach (var id in newWords) data.State.CurrentIterationQueue.Enqueue(id);
                foreach (var id in learningWords) data.State.CurrentIterationQueue.Enqueue(id);
                foreach (var id in learnedReview) data.State.CurrentIterationQueue.Enqueue(id);
            }

            var nextWordId = data.State.CurrentIterationQueue.Dequeue();
            data.Session.CurrentWordId = nextWordId;
            _db.SaveChanges();

            var promptTerm = _db.VocabTerms
                .Where(t => t.VocabItemId == nextWordId && t.LanguageCode == sourceLanguage)
                .Select(t => t.Text)
                .First();

            var prompt = promptTerm;

            return new GetNextPracticeWordResult(nextWordId, prompt);
        }
    }
}
