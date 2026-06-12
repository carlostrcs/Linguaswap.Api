using LinguaSwap.Application.Practice.Common;
using LinguaSwap.Domain.Practice;
using LinguaSwap.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Practice.StartPracticeSession
{
    public sealed class StartPracticeSessionHandler
    {
        private readonly InMemoryPracticeSessionStore _store;
        private readonly LinguaSwapDbContext _db;

        public StartPracticeSessionHandler(InMemoryPracticeSessionStore store, LinguaSwapDbContext db)
        {
            _store = store;
            _db = db;
        }

        public StartPracticeSessionResult Handle(StartPracticeSessionCommand command)
        {
            var sourceLanguage = command.SourceLanguage.Trim().ToLowerInvariant();
            var targetLanguage = command.TargetLanguage.Trim().ToLowerInvariant();

            if (sourceLanguage.Length == 0 || targetLanguage.Length == 0)
                throw new InvalidOperationException("Source and target languages are required.");

            if (sourceLanguage == targetLanguage)
                throw new InvalidOperationException("Source and target languages must be different.");

            if (command.LibraryId is Guid libraryId)
            {
                var allowed = _db.Libraries.Any(l =>
                    l.Id == libraryId &&
                    (l.IsPublic || (command.UserId != null && l.UserId == command.UserId)));

                if (!allowed)
                    throw new UnauthorizedAccessException("Not allowed to use this library.");

                var hasPracticeableItems = _db.VocabItems
                    .Where(i => i.LibraryId == libraryId)
                    .Any(i =>
                        _db.VocabTerms.Any(t =>
                            t.VocabItemId == i.Id &&
                            t.LanguageCode == sourceLanguage)
                        &&
                        _db.VocabTerms.Any(t =>
                            t.VocabItemId == i.Id &&
                            t.LanguageCode == targetLanguage)
                    );

                if (!hasPracticeableItems)
                    throw new InvalidOperationException(
                        "This library does not have enough vocabulary for this practice session."
                    );
            }

            var session = new PracticeSession
            {
                UserId = command.UserId,
                LibraryId = command.LibraryId,
                SourceLanguage = sourceLanguage,
                TargetLanguage = targetLanguage,
                Direction = command.Direction,
                Difficulty = command.Difficulty
            };

            _db.PracticeSessions.Add(session);
            _db.SaveChanges();

            _store.Add(session);

            return new StartPracticeSessionResult(session.Id);
        }
    }
}
