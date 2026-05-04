using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LinguaSwap.Domain.Users;
using LinguaSwap.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
namespace LinguaSwap.Application.Auth.Login
{
    public sealed class LoginHandler
    {
        private readonly LinguaSwapDbContext _db;
        private readonly IConfiguration _config;

        public LoginHandler(LinguaSwapDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public LoginResult Handle(LoginCommand command)
        {
            var email = command.Email.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(command.Password))
                throw new InvalidOperationException("Email and password are required.");

            var user = _db.Users.SingleOrDefault(u => u.Email == email);
            if (user is null) throw new UnauthorizedAccessException("Invalid credentials.");

            var ok = BCrypt.Net.BCrypt.Verify(command.Password, user.PasswordHash);
            if (!ok) throw new UnauthorizedAccessException("Invalid credentials.");

            var token = CreateJwt(user);
            return new LoginResult(token);
        }

        private string CreateJwt(User user)
        {
            var key = _config["Jwt:Key"]!;
            var issuer = _config["Jwt:Issuer"]!;
            var audience = _config["Jwt:Audience"]!;

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
        };

            var creds = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
