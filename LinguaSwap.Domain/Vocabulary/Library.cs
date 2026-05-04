using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Domain.Vocabulary
{
    public sealed class Library
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        public Guid? UserId { get; set; }

        public string Name { get; set; } = string.Empty;
        public bool IsPublic { get; set; } = false;
    }
}
