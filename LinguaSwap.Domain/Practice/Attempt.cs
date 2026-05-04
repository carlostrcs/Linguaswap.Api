using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Domain.Practice
{
    public sealed class Attempt
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        public Guid SessionId { get; init; }
        public Guid WordId { get; init; }

        public string UserAnswer { get; init; } = string.Empty;
        public bool IsCorrect { get; init; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
