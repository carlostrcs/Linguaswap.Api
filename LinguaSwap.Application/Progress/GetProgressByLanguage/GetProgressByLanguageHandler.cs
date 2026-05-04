using LinguaSwap.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Progress.GetProgressByLanguage
{
    public sealed class GetProgressByLanguageHandler
    {
        private readonly LinguaSwapDbContext _db;
        public GetProgressByLanguageHandler(LinguaSwapDbContext db) => _db = db;

        public List<GetProgressByLanguageResult> Handle(GetProgressByLanguageQuery query)
        {
            var rows = _db.UserVocabStats
                .Where(s => s.UserId == query.UserId)
                .GroupBy(s => s.TargetLanguage)
                .Select(g => new
                {
                    TargetLanguage = g.Key,
                    DistinctWords = g.Count(),
                    CorrectAttempts = g.Sum(x => x.CorrectCount),
                    WrongAttempts = g.Sum(x => x.WrongCount)
                })
                .ToList();

            return rows
                .Select(x =>
                {
                    var total = x.CorrectAttempts + x.WrongAttempts;
                    var acc = total == 0 ? 0 : (double)x.CorrectAttempts / total;

                    return new GetProgressByLanguageResult(
                        x.TargetLanguage,
                        x.DistinctWords,
                        total,
                        x.CorrectAttempts,
                        acc
                    );
                })
                .OrderByDescending(x => x.TotalAttempts)
                .ToList();
        }
    }
}
