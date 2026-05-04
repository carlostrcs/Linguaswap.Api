using LinguaSwap.Domain.Vocabulary;
using LinguaSwap.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.CreateVocabItem
{
    public sealed class CreateVocabItemHandler
    {
        private readonly LinguaSwapDbContext _db;

        public CreateVocabItemHandler(LinguaSwapDbContext db)
        {
            _db = db;
        }

        public CreateVocabItemResult Handle(CreateVocabItemCommand command)
        {
            var lib = _db.Libraries
                .Where(l => l.Id == command.LibraryId)
                .Select(l => new { l.UserId, l.IsPublic })
                .SingleOrDefault();

            if (lib is null)
                throw new KeyNotFoundException("Library not found.");

            if (lib.UserId != command.UserId)
                throw new UnauthorizedAccessException("Not allowed.");

            if (command.Terms is null || command.Terms.Count < 2)
                throw new InvalidOperationException("Provide at least 2 terms");

            var normalized = command.Terms
                .Select(t => new TermInput(
                    LanguageCode: t.LanguageCode.Trim().ToLowerInvariant(),
                    Text: t.Text.Trim()
                ))
                .Where(t => t.LanguageCode.Length > 0 && t.Text.Length > 0)
                .ToList();

            if (normalized.Count < 2)
                throw new InvalidOperationException("Provide at least 2 valid terms");

            var item = new VocabItem
            {
                LibraryId = command.LibraryId
            };

            _db.VocabItems.Add(item);
            _db.SaveChanges();

            var terms = normalized.Select(t => new VocabTerm
            {
                VocabItemId = item.Id,
                LanguageCode = t.LanguageCode,
                Text = t.Text
            });

            _db.VocabTerms.AddRange(terms);
            _db.SaveChanges();

            return new CreateVocabItemResult(item.Id);
        }
    }
}
