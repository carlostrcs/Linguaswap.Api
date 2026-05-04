using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Domain.Practice
{
    public sealed class UserVocabStats
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        public Guid UserId { get; init; }

        public Guid VocabItemId { get; init; }

        public string SourceLanguage { get; init; } = string.Empty;

        public string TargetLanguage { get; init; } = string.Empty;

        public int CorrectCount { get; set; }

        public int WrongCount { get; set; }

        public DateTime LastPracticedAt { get; set; } = DateTime.UtcNow;
    }
}
