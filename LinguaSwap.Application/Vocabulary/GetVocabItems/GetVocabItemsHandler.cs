using LinguaSwap.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.GetVocabItems
{
    public sealed class GetVocabItemsHandler
    {
        private readonly LinguaSwapDbContext _db;
        public GetVocabItemsHandler(LinguaSwapDbContext db) => _db = db;

        public List<GetVocabItemsResult> Handle(GetVocabItemsQuery query)
        {
            var baseQuery =
                from item in _db.VocabItems
                join lib in _db.Libraries on item.LibraryId equals lib.Id
                where lib.UserId == query.UserId
                select new { item, lib };

            if (query.LibraryId is Guid libraryId)
                baseQuery = baseQuery.Where(x => x.item.LibraryId == libraryId);

            // term count
            var result =
                from x in baseQuery
                join term in _db.VocabTerms on x.item.Id equals term.VocabItemId into terms
                select new GetVocabItemsResult(
                    Id: x.item.Id,
                    LibraryId: x.item.LibraryId,
                    TermCount: terms.Count()
                );

            return result.ToList();
        }
    }
}
