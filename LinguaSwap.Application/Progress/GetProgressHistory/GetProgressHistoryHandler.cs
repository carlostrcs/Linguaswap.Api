using LinguaSwap.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
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

            var grouped =
                (from a in _db.Attempts.AsNoTracking()
                 join s in _db.PracticeSessions.AsNoTracking()
                    on a.SessionId equals s.Id
                 where s.UserId == query.UserId
                    && s.SourceLanguage == source
                    && s.TargetLanguage == target
                    && a.CreatedAt >= fromDate
                 group a by new
                 {
                     a.CreatedAt.Year,
                     a.CreatedAt.Month,
                     a.CreatedAt.Day
                 }
                 into g
                 orderby g.Key.Year, g.Key.Month, g.Key.Day
                 select new
                 {
                     g.Key.Year,
                     g.Key.Month,
                     g.Key.Day,
                     Attempts = g.Count(),
                     Correct = g.Sum(x => x.IsCorrect ? 1 : 0)
                 })
                .ToList();

            return grouped
                .Select(x => new GetProgressHistoryResult(
                    Day: new DateTime(x.Year, x.Month, x.Day),
                    Attempts: x.Attempts,
                    Correct: x.Correct
                ))
                .ToList();
        }
    }
}
