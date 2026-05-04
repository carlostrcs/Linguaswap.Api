using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Domain.Practice
{
    public sealed class PracticeSession
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public Guid? UserId { get; init; } // null = demo
        public DateTimeOffset StartedAt { get; init; } = DateTimeOffset.UtcNow;

        public PracticeDirection Direction { get; init; }
        public PracticeDifficulty Difficulty { get; init; }
        public Guid? LibraryId { get; init; }

        public string SourceLanguage { get; init; } = string.Empty;

        public string TargetLanguage { get; init; } = string.Empty;
        public Guid? CurrentWordId { get; set; }
    }
}
