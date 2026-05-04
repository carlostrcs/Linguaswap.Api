using LinguaSwap.Infrastructure.Persistence;
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

            return _db.UserVocabStats
                .Where(s =>
                    s.UserId == query.UserId &&
                    s.SourceLanguage == source &&
                    s.TargetLanguage == target &&
                    (s.CorrectCount + s.WrongCount) > 0)
                // ✅ Ordenar por accuracy SIN crear DTO antes
                .OrderBy(s => (double)s.CorrectCount / (s.CorrectCount + s.WrongCount))
                .ThenByDescending(s => s.WrongCount)
                .Take(limit)
                // ✅ Ahora sí, proyecta
                .Select(s => new GetTopMistakesResult(
                    s.VocabItemId,
                    s.CorrectCount,
                    s.WrongCount,
                    (double)s.CorrectCount / (s.CorrectCount + s.WrongCount)
                ))
                .ToList();
        }
    }
}
