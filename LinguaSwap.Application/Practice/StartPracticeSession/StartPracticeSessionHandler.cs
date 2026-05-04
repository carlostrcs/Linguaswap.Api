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
            if (command.LibraryId is Guid libraryId)
            {
                var allowed = _db.Libraries.Any(l =>
                    l.Id == libraryId &&
                    (l.IsPublic || (command.UserId != null && l.UserId == command.UserId)));

                if (!allowed)
                    throw new UnauthorizedAccessException("Not allowed to use this library.");
            }

            var session = new PracticeSession
            {
                UserId = command.UserId,
                LibraryId = command.LibraryId,
                SourceLanguage = command.SourceLanguage.ToLowerInvariant(),
                TargetLanguage = command.TargetLanguage.ToLowerInvariant(),
                Direction = command.Direction,
                Difficulty = command.Difficulty
            };

            // 1) Persistir en Postgres
            _db.PracticeSessions.Add(session);
            _db.SaveChanges();

            // 2) Mantener en memoria el estado de la sesión (cola, set, stats)
            _store.Add(session);

            return new StartPracticeSessionResult(session.Id);
        }
    }
}
