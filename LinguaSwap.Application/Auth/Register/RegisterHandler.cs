using LinguaSwap.Domain.Users;
using LinguaSwap.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSwap.Application.Auth.Register
{
    public sealed class RegisterHandler
    {
        private readonly LinguaSwapDbContext _db;

        public RegisterHandler(LinguaSwapDbContext db) => _db = db;

        public RegisterResult Handle(RegisterCommand command)
        {
            var email = command.Email.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(command.Password))
                throw new InvalidOperationException("Email and password are required.");

            if (_db.Users.Any(u => u.Email == email))
                throw new InvalidOperationException("Email already registered.");

            var user = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password)
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return new RegisterResult(user.Id, user.Email);
        }
    }
}
