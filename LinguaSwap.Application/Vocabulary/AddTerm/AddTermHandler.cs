using LinguaSwap.Domain.Vocabulary;
using LinguaSwap.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.AddTerm
{
    public sealed class AddTermHandler
    {
        private readonly LinguaSwapDbContext _db;

        public AddTermHandler(LinguaSwapDbContext db) => _db = db;

        public AddTermResult Handle(AddTermCommand command)
        {
            var language = command.LanguageCode.Trim().ToLowerInvariant();
            var text = command.Text.Trim();

            if (string.IsNullOrWhiteSpace(language) || string.IsNullOrWhiteSpace(text))
                throw new InvalidOperationException("LanguageCode and Text are required.");

            // Cargar item + library para ownership
            var item = _db.VocabItems
                .Join(_db.Libraries, i => i.LibraryId, l => l.Id, (i, l) => new { Item = i, Library = l })
                .SingleOrDefault(x => x.Item.Id == command.VocabItemId);

            if (item is null)
                throw new InvalidOperationException("VocabItem not found.");

            if (item.Library.UserId != command.UserId)
                throw new UnauthorizedAccessException("Not allowed.");

            // Evitar duplicados exactos (mismo item, mismo idioma, mismo texto)
            var duplicate = _db.VocabTerms.Any(t =>
                t.VocabItemId == command.VocabItemId &&
                t.LanguageCode == language &&
                t.Text == text);

            if (duplicate)
                throw new InvalidOperationException("Term already exists.");

            var term = new VocabTerm
            {
                VocabItemId = command.VocabItemId,
                LanguageCode = language,
                Text = text
            };

            _db.VocabTerms.Add(term);
            _db.SaveChanges();

            return new AddTermResult(term.Id);
        }
    }
}
