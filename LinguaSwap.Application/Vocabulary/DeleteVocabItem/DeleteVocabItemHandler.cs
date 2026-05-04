using LinguaSwap.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.DeleteVocabItem
{
    public sealed class DeleteVocabItemHandler
    {
        private readonly LinguaSwapDbContext _db;

        public DeleteVocabItemHandler(LinguaSwapDbContext db) => _db = db;

        public void Handle(DeleteVocabItemCommand command)
        {
            // item + library (ownership)
            var data = _db.VocabItems
                .Where(i => i.Id == command.VocabItemId)
                .Join(_db.Libraries, i => i.LibraryId, l => l.Id, (i, l) => new { Item = i, Library = l })
                .SingleOrDefault();

            if (data is null)
                throw new InvalidOperationException("VocabItem not found.");

            if (data.Library.UserId != command.UserId)
                throw new UnauthorizedAccessException("Not allowed.");

            _db.VocabItems.Remove(data.Item);
            _db.SaveChanges();
        }
    }
}
