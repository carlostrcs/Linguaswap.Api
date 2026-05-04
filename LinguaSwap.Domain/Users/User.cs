using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Domain.Users
{
    public sealed class User
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        public string Email { get; set; } = string.Empty;

        // Guardamos hash, nunca password en claro
        public string PasswordHash { get; set; } = string.Empty;

        public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    }
}
