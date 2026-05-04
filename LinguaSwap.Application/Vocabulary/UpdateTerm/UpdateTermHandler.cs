using LinguaSwap.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Vocabulary.UpdateTerm
{
    public sealed class UpdateTermHandler
    {
        private readonly LinguaSwapDbContext _db;

        public UpdateTermHandler(LinguaSwapDbContext db) => _db = db;

        public UpdateTermResult Handle(UpdateTermCommand command)
        {
            var language = command.LanguageCode.Trim().ToLowerInvariant();
            var text = command.Text.Trim();

            if (string.IsNullOrWhiteSpace(language) || string.IsNullOrWhiteSpace(text))
                throw new InvalidOperationException("LanguageCode and Text are required.");

            // term + item + library (ownership)
            var data = _db.VocabTerms
                .Where(t => t.Id == command.TermId)
                .Join(_db.VocabItems, t => t.VocabItemId, i => i.Id, (t, i) => new { Term = t, Item = i })
                .Join(_db.Libraries, ti => ti.Item.LibraryId, l => l.Id, (ti, l) => new { ti.Term, ti.Item, Library = l })
                .SingleOrDefault();

            if (data is null)
                throw new InvalidOperationException("Term not found.");

            if (data.Library.UserId != command.UserId)
                throw new UnauthorizedAccessException("Not allowed.");

            // evitar duplicado exacto dentro del mismo item
            var duplicateExists = _db.VocabTerms.Any(t =>
                t.VocabItemId == data.Item.Id &&
                t.Id != data.Term.Id &&
                t.LanguageCode == language &&
                t.Text == text);

            if (duplicateExists)
                throw new InvalidOperationException("A term with the same language and text already exists for this item.");

            data.Term.LanguageCode = language;
            data.Term.Text = text;

            _db.SaveChanges();

            return new UpdateTermResult(data.Term.Id);
        }
    }
}
