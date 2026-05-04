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
            // 1) Encontrar term + su item + su library
            var term = _db.VocabTerms
                .Where(t => t.Id == command.TermId)
                .Join(_db.VocabItems, t => t.VocabItemId, i => i.Id, (t, i) => new { t, i })
                .Join(_db.Libraries, ti => ti.i.LibraryId, l => l.Id, (ti, l) => new { ti.t, Library = l })
                .SingleOrDefault();

            if (term is null)
                throw new InvalidOperationException("Term not found.");

            // 2) Ownership
            if (term.Library.UserId != command.UserId)
                throw new UnauthorizedAccessException("Not allowed.");

            // 3) Delete
            _db.VocabTerms.Remove(term.t);
            _db.SaveChanges();
        }
    }
}
