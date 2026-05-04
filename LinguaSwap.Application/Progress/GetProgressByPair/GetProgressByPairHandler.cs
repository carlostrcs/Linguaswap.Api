using LinguaSwap.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Progress.GetProgressByPair
{
    public sealed class GetProgressByPairHandler
    {
        private readonly LinguaSwapDbContext _db;
        public GetProgressByPairHandler(LinguaSwapDbContext db) => _db = db;

        public List<GetProgressByPairResult> Handle(GetProgressByPairQuery query)
        {
            // Traemos agregado de BD y calculamos Accuracy en memoria para evitar líos de EF con división
            var rows = _db.UserVocabStats
                .Where(s => s.UserId == query.UserId)
                .GroupBy(s => new { s.SourceLanguage, s.TargetLanguage })
                .Select(g => new
                {
                    g.Key.SourceLanguage,
                    g.Key.TargetLanguage,
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

                    return new GetProgressByPairResult(
                        x.SourceLanguage,
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
