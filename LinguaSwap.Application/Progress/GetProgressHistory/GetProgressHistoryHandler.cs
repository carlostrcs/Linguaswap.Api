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

            if (source.Length == 0 || target.Length == 0)
                throw new InvalidOperationException("Source and target languages are required.");

            if (source == target)
                throw new InvalidOperationException("Source and target languages must be different.");

            var today = DateTime.UtcNow.Date;
            var fromDate = today.AddDays(-days + 1);

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

            var statsByDay = grouped.ToDictionary(
                x => new DateTime(x.Year, x.Month, x.Day),
                x => new
                {
                    x.Attempts,
                    x.Correct
                }
            );

            return Enumerable
                .Range(0, days)
                .Select(offset =>
                {
                    var day = fromDate.AddDays(offset);

                    if (!statsByDay.TryGetValue(day, out var stats))
                    {
                        return new GetProgressHistoryResult(
                            Day: day,
                            TotalAttempts: 0,
                            CorrectAttempts: 0,
                            IncorrectAttempts: 0,
                            Accuracy: 0
                        );
                    }

                    var incorrect = stats.Attempts - stats.Correct;

                    var accuracy = stats.Attempts == 0
                        ? 0
                        : (double)stats.Correct / stats.Attempts;

                    return new GetProgressHistoryResult(
                        Day: day,
                        TotalAttempts: stats.Attempts,
                        CorrectAttempts: stats.Correct,
                        IncorrectAttempts: incorrect,
                        Accuracy: accuracy
                    );
                })
                .ToList();
        }
    }
}
