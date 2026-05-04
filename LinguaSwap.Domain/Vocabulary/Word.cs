using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Domain.Vocabulary
{
    public sealed class Word
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Native { get; init; } = string.Empty;
        public string Target { get; init; } = string.Empty;
    }
}
