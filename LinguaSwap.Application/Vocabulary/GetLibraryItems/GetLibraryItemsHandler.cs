using LinguaSwap.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.GetLibraryItems
{
    public sealed class GetLibraryItemsHandler
    {
        private readonly LinguaSwapDbContext _db;

        public GetLibraryItemsHandler(LinguaSwapDbContext db)
        {
            _db = db;
        }

        public GetLibraryItemsResult Handle(GetLibraryItemsQuery query)
        {
            var lib = _db.Libraries
                .Where(l => l.Id == query.LibraryId)
                .Select(l => new { l.UserId, l.IsPublic })
                .SingleOrDefault()
                ?? throw new KeyNotFoundException("Library not found.");

            var allowed = lib.IsPublic || (query.UserId is Guid uid && lib.UserId == uid);
            if (!allowed)
                throw new UnauthorizedAccessException("Not allowed.");

            var items = _db.VocabItems
                .Where(i => i.LibraryId == query.LibraryId)
                .Select(i => new GetLibraryItemsItem(
                    i.Id,
                    _db.VocabTerms
                        .Where(t => t.VocabItemId == i.Id)
                        .OrderBy(t => t.LanguageCode)
                        .Select(t => new GetLibraryItemsTerm(t.Id, t.LanguageCode, t.Text))
                        .ToList()
                ))
                .ToList();

            return new GetLibraryItemsResult(items);
        }
    }
}
