using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Domain.Vocabulary
{
    public sealed class VocabTerm
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        public Guid VocabItemId { get; init; }

        // ISO 639-1 recomendado: "es", "en", "fr", "de", etc.
        public string LanguageCode { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}
