using LinguaSwap.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.GetPublicLibraries
{
    public sealed class GetPublicLibrariesHandler
    {
        private readonly LinguaSwapDbContext _db;

        public GetPublicLibrariesHandler(LinguaSwapDbContext db)
        {
            _db = db;
        }

        public GetPublicLibrariesResult Handle(GetPublicLibrariesQuery query)
        {
            var items = _db.Libraries
                .Where(l => l.IsPublic)
                .OrderBy(l => l.Name)
                .Select(l => new GetPublicLibrariesItem(l.Id, l.Name))
                .ToList();

            return new GetPublicLibrariesResult(items);
        }
    }
}
