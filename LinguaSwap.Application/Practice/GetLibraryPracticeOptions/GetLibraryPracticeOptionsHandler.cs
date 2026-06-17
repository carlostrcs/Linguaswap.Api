using LinguaSwap.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Practice.GetLibraryPracticeOptions
{
    public sealed class GetLibraryPracticeOptionsHandler
    {
        private readonly LinguaSwapDbContext _db;

        public GetLibraryPracticeOptionsHandler(LinguaSwapDbContext db)
        {
            this._db = db;
        }

        public GetLibraryPracticeOptionsResult Handle(GetLibraryPracticeOptionsQuery query)
        {
            var canUseLibrary = _db.Libraries
            .AsNoTracking()
            .Any(l =>
                l.Id == query.LibraryId &&
                (
                    l.IsPublic ||
                    query.UserId != null && l.UserId == query.UserId
                ));

            if (!canUseLibrary)
                throw new UnauthorizedAccessException("Not allowed to use this library.");

            var terms = _db.VocabTerms
            .AsNoTracking()
            .Where(t =>
                _db.VocabItems.Any(i =>
                    i.Id == t.VocabItemId &&
                    i.LibraryId == query.LibraryId))
            .Select(t => new
            {
                t.VocabItemId,
                LanguageCode = t.LanguageCode.ToLower()
            })
            .Distinct()
            .ToList();

            var languages = terms
                .Select(t => t.LanguageCode)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var languageSetsByItem = terms
                .GroupBy(t => t.VocabItemId)
                .Select(g => g
                    .Select(x => x.LanguageCode)
                    .Distinct()
                    .ToHashSet())
                .ToList();

            var pairs = new List<PracticeLanguagePairResult>();

            foreach (var sourceLanguage in languages)
            {
                foreach (var targetLanguage in languages)
                {
                    if (sourceLanguage == targetLanguage)
                        continue;

                    var vocabItemCount = languageSetsByItem.Count(languageSet =>
                        languageSet.Contains(sourceLanguage) &&
                        languageSet.Contains(targetLanguage));

                    if (vocabItemCount > 0)
                    {
                        pairs.Add(new PracticeLanguagePairResult(
                            sourceLanguage,
                            targetLanguage,
                            vocabItemCount
                        ));
                    }
                }
            }

            return new GetLibraryPracticeOptionsResult(
                query.LibraryId,
                languages,
                pairs
            );

        }
    }
}
