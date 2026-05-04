using LinguaSwap.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.GetLibraries
{
    public sealed class GetLibrariesHandler
    {
        private readonly LinguaSwapDbContext _db;
        public GetLibrariesHandler(LinguaSwapDbContext db) => _db = db;

        public List<GetLibrariesResult> Handle(GetLibrariesQuery query)
        {
            return _db.Libraries
                .Where(l => l.UserId == query.UserId)
                .OrderBy(l => l.Name)
                .Select(l => new GetLibrariesResult(l.Id, l.Name))
                .ToList();
        }
    }
}
