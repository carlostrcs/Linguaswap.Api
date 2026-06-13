using LinguaSwap.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Progress.GetProgressSummary
{
    public sealed class GetProgressSummaryHandler
    {
        private readonly LinguaSwapDbContext _db;

        public GetProgressSummaryHandler(LinguaSwapDbContext db)
        {
            _db = db;
        }

        public GetProgressSummaryResult Handle(GetProgressSummaryQuery query)
        {
            var stats = _db.UserVocabStats
                .Where(s =>
                    s.UserId == query.UserId &&
                    s.SourceLanguage == query.SourceLanguage &&
                    s.TargetLanguage == query.TargetLanguage)
                .ToList();

            var correctAttempts = stats.Sum(s => s.CorrectCount);
            var incorrectAttempts = stats.Sum(s => s.WrongCount);
            var totalAttempts = correctAttempts + incorrectAttempts;

            var accuracy = totalAttempts == 0
                ? 0
                : (double)correctAttempts / totalAttempts;

            return new GetProgressSummaryResult(
                TotalAttempts: totalAttempts,
                CorrectAttempts: correctAttempts,
                IncorrectAttempts: incorrectAttempts,
                Accuracy: accuracy,
                DistinctWords: stats.Count
            );
        }
    }
}
