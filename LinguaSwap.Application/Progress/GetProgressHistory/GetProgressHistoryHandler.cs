using LinguaSwap.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Progress.GetProgressHistory
{
    public sealed class GetProgressHistoryHandler
    {
        private readonly LinguaSwapDbContext _db;
        public GetProgressHistoryHandler(LinguaSwapDbContext db) => _db = db;

        public List<GetProgressHistoryResult> Handle(GetProgressHistoryQuery query)
        {
            var source = query.SourceLanguage.Trim().ToLowerInvariant();
            var target = query.TargetLanguage.Trim().ToLowerInvariant();
            var days = Math.Clamp(query.Days, 1, 365);

            var fromDate = DateTime.UtcNow.Date.AddDays(-days + 1);

            // JOIN Attempt -> PracticeSession por SessionId
            var joined =
                from a in _db.Attempts
                join s in _db.PracticeSessions on a.SessionId equals s.Id
                where s.UserId == query.UserId
                   && s.SourceLanguage == source
                   && s.TargetLanguage == target
                   && a.CreatedAt >= fromDate
    
                select new { a.CreatedAt, a.IsCorrect };

            return joined
                .GroupBy(x => x.CreatedAt.Date)
                .Select(g => new GetProgressHistoryResult(
                    Day: g.Key,
                    Attempts: g.Count(),
                    Correct: g.Count(x => x.IsCorrect)
                ))
                .OrderBy(x => x.Day)
                .ToList();
        }
    }
}
