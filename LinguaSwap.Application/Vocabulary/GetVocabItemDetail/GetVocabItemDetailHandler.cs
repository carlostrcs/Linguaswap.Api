using LinguaSwap.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.GetVocabItemDetail
{
    public sealed class GetVocabItemDetailHandler
    {
        private readonly LinguaSwapDbContext _db;
        public GetVocabItemDetailHandler(LinguaSwapDbContext db) => _db = db;

        public GetVocabItemDetailResult Handle(GetVocabItemDetailQuery query)
        {
            // Ownership: item -> library -> user
            var itemWithLibrary = _db.VocabItems
                .Where(i => i.Id == query.VocabItemId)
                .Join(_db.Libraries, i => i.LibraryId, l => l.Id, (i, l) => new { Item = i, Library = l })
                .SingleOrDefault();

            if (itemWithLibrary is null)
                throw new InvalidOperationException("VocabItem not found.");

            if (itemWithLibrary.Library.UserId != query.UserId)
                throw new UnauthorizedAccessException("Not allowed.");

            var terms = _db.VocabTerms
                .Where(t => t.VocabItemId == query.VocabItemId)
                .OrderBy(t => t.LanguageCode)
                .ThenBy(t => t.Text)
                .Select(t => new VocabTermDto(t.Id, t.LanguageCode, t.Text))
                .ToList();

            return new GetVocabItemDetailResult(
                Id: itemWithLibrary.Item.Id,
                LibraryId: itemWithLibrary.Item.LibraryId,
                Terms: terms
            );
        }
    }
}
