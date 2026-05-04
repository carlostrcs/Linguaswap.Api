using LinguaSwap.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.DeleteLibrary
{
    public sealed class DeleteLibraryHandler
    {
        private readonly LinguaSwapDbContext _db;

        public DeleteLibraryHandler(LinguaSwapDbContext db) => _db = db;

        public void Handle(DeleteLibraryCommand command)
        {
            var library = _db.Libraries.SingleOrDefault(l => l.Id == command.LibraryId);
            if (library is null)
                throw new InvalidOperationException("Library not found.");

            if (library.UserId != command.UserId)
                throw new UnauthorizedAccessException("Not allowed.");

            _db.Libraries.Remove(library);
            _db.SaveChanges();
        }
    }
}
