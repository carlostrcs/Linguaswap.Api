using LinguaSwap.Domain.Vocabulary;
using LinguaSwap.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.CreateLibrary
{
    public sealed class CreateLibraryHandler
    {
        private readonly LinguaSwapDbContext _db;

        public CreateLibraryHandler(LinguaSwapDbContext db)
        {
            _db = db;
        }

        public CreateLibraryResult Handle(CreateLibraryCommand command)
        {
            var name = command.Name.Trim();
            if (name.Length == 0) throw new InvalidOperationException("Library name is required");
            if (name.Length > 120) throw new InvalidOperationException("Library name too long");

            var library = new Library
            {
                UserId = command.UserId,
                Name = name
            };

            _db.Libraries.Add(library);
            _db.SaveChanges();

            return new CreateLibraryResult(library.Id);
        }
    }
}
