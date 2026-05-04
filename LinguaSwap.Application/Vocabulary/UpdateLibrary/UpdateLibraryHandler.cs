using LinguaSwap.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.UpdateLibrary
{
    public sealed class UpdateLibraryHandler
    {
        private readonly LinguaSwapDbContext _db;
        public UpdateLibraryHandler(LinguaSwapDbContext db) => _db = db;

        public UpdateLibraryResult Handle(UpdateLibraryCommand command)
        {
            var name = command.Name.Trim();
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("Name is required.");

            var library = _db.Libraries.SingleOrDefault(l => l.Id == command.LibraryId);
            if (library is null)
                throw new InvalidOperationException("Library not found.");

            if (library.UserId != command.UserId)
                throw new UnauthorizedAccessException("Not allowed.");

            library.Name = name;
            _db.SaveChanges();

            return new UpdateLibraryResult(library.Id, library.Name);
        }
    }
}
