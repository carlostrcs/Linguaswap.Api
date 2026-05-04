using LinguaSwap.Domain.Practice;
using LinguaSwap.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Practice.Common
{
    public sealed class InMemoryPracticeSessionStore
    {
        public sealed class SessionData
        {
            public PracticeSession Session { get; init; } = default!;
            public PracticeSessionState State { get; init; } = new();
            public List<Attempt> Attempts { get; } = new();
        }

        private readonly Dictionary<Guid, SessionData> _sessions = new();

        public void Add(PracticeSession session)
        {
            _sessions[session.Id] = new SessionData { Session = session };
        }

        public SessionData? Get(Guid sessionId)
        {
            _sessions.TryGetValue(sessionId, out var data);
            return data;
        }

        public void EnsureLoaded(Guid sessionId, LinguaSwapDbContext db)
        {
            if (_sessions.ContainsKey(sessionId)) return;

            var session = db.PracticeSessions.SingleOrDefault(s => s.Id == sessionId);
            if (session is null) throw new InvalidOperationException("Session not found");

            var data = new SessionData { Session = session };

            // Reconstruir stats desde attempts guardados
            var attempts = db.Attempts.Where(a => a.SessionId == sessionId).ToList();
            data.Attempts.AddRange(attempts);

            foreach (var a in attempts)
            {
                data.State.StatsByWordId.TryAdd(a.WordId, new WordPracticeStats());
                data.State.StatsByWordId[a.WordId].TimesShown++; // aproximación (1 attempt = 1 shown)
                if (a.IsCorrect) data.State.StatsByWordId[a.WordId].CorrectCount++;
            }

            _sessions[sessionId] = data;
        }
    }
}
