using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Domain.Vocabulary
{
    public sealed class VocabItem
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        public Guid LibraryId { get; init; }
    }
}
