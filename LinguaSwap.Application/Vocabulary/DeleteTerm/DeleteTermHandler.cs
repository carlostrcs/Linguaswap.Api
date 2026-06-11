using LinguaSwap.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.DeleteTerm
{
    public sealed class DeleteTermHandler
    {
        private readonly LinguaSwapDbContext _db;

        public DeleteTermHandler(LinguaSwapDbContext db) => _db = db;

        public void Handle(DeleteTermCommand command)
        {
            var term = _db.VocabTerms
               .Where(t => t.Id == command.TermId)
               .Join(
                   _db.VocabItems,
                   t => t.VocabItemId,
                   i => i.Id,
                   (t, i) => new { Term = t, Item = i }
               )
               .Join(
                   _db.Libraries,
                   ti => ti.Item.LibraryId,
                   l => l.Id,
                   (ti, l) => new
                   {
                       ti.Term,
                       VocabItemId = ti.Item.Id,
                       LibraryUserId = l.UserId
                   }
               )
               .SingleOrDefault();

            if (term is null)
                throw new InvalidOperationException("Term not found.");

            if (term.LibraryUserId != command.UserId)
                throw new UnauthorizedAccessException("Not allowed.");

            var termsCount = _db.VocabTerms
                .Count(t => t.VocabItemId == term.VocabItemId);

            if (termsCount <= 2)
                throw new InvalidOperationException(
                    "A vocabulary item must have at least 2 terms."
                );

            _db.VocabTerms.Remove(term.Term);
            _db.SaveChanges();
        }
    }
}
