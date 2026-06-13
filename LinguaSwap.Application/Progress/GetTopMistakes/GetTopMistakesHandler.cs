using LinguaSwap.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Progress.GetTopMistakes
{
    public sealed class GetTopMistakesHandler
    {
        private readonly LinguaSwapDbContext _db;
        public GetTopMistakesHandler(LinguaSwapDbContext db) => _db = db;

        public List<GetTopMistakesResult> Handle(GetTopMistakesQuery query)
        {
            var source = query.SourceLanguage.Trim().ToLowerInvariant();
            var target = query.TargetLanguage.Trim().ToLowerInvariant();
            var limit = Math.Clamp(query.Limit, 1, 100);

            if (source.Length == 0 || target.Length == 0)
                throw new InvalidOperationException("Source and target languages are required.");

            if (source == target)
                throw new InvalidOperationException("Source and target languages must be different.");

            return _db.UserVocabStats
                .AsNoTracking()
                .Where(s =>
                    s.UserId == query.UserId &&
                    s.SourceLanguage == source &&
                    s.TargetLanguage == target &&
                    s.WrongCount > 0)
                .Select(s => new
                {
                    s.VocabItemId,
                    CorrectAttempts = s.CorrectCount,
                    IncorrectAttempts = s.WrongCount,
                    TotalAttempts = s.CorrectCount + s.WrongCount,
                    Accuracy = (double)s.CorrectCount / (s.CorrectCount + s.WrongCount),

                    SourceText = _db.VocabTerms
                        .Where(t =>
                            t.VocabItemId == s.VocabItemId &&
                            t.LanguageCode == source)
                        .Select(t => t.Text)
                        .FirstOrDefault(),

                    TargetText = _db.VocabTerms
                        .Where(t =>
                            t.VocabItemId == s.VocabItemId &&
                            t.LanguageCode == target)
                        .Select(t => t.Text)
                        .FirstOrDefault()
                })
                .Where(x => x.SourceText != null && x.TargetText != null)
                .OrderBy(x => x.Accuracy)
                .ThenByDescending(x => x.IncorrectAttempts)
                .ThenByDescending(x => x.TotalAttempts)
                .Take(limit)
                .Select(x => new GetTopMistakesResult(
                    x.VocabItemId,
                    x.SourceText!,
                    x.TargetText!,
                    x.TotalAttempts,
                    x.CorrectAttempts,
                    x.IncorrectAttempts,
                    x.Accuracy
                ))
                .ToList();
        }
    }
}
