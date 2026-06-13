using LinguaSwap.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
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
            var rows = _db.UserVocabStats
                .AsNoTracking()
                .Where(s =>
                    s.UserId == query.UserId &&
                    (s.CorrectCount + s.WrongCount) > 0)
                .GroupBy(s => new
                {
                    s.SourceLanguage,
                    s.TargetLanguage
                })
                .Select(g => new
                {
                    g.Key.SourceLanguage,
                    g.Key.TargetLanguage,
                    DistinctWords = g.Count(),
                    CorrectAttempts = g.Sum(x => x.CorrectCount),
                    IncorrectAttempts = g.Sum(x => x.WrongCount)
                })
                .ToList();

            return rows
                .Select(x =>
                {
                    var totalAttempts = x.CorrectAttempts + x.IncorrectAttempts;

                    var accuracy = totalAttempts == 0
                        ? 0
                        : (double)x.CorrectAttempts / totalAttempts;

                    return new GetProgressByPairResult(
                        SourceLanguage: x.SourceLanguage,
                        TargetLanguage: x.TargetLanguage,
                        DistinctWords: x.DistinctWords,
                        TotalAttempts: totalAttempts,
                        CorrectAttempts: x.CorrectAttempts,
                        IncorrectAttempts: x.IncorrectAttempts,
                        Accuracy: accuracy
                    );
                })
                .OrderByDescending(x => x.TotalAttempts)
                .ThenBy(x => x.SourceLanguage)
                .ThenBy(x => x.TargetLanguage)
                .ToList();
        }
    }
}
