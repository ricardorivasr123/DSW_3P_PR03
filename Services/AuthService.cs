using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CharacterHub.DTOs;
using Microsoft.IdentityModel.Tokens;

namespace CharacterHub.Services;

public sealed class AuthService(IConfiguration configuration) : IAuthService
{
    public LoginResponse? Login(string username, string password)
    {
        var expectedUsername = configuration["DemoUser:Username"]!;
        var expectedPassword = configuration["DemoUser:Password"]!;
        if (!string.Equals(username, expectedUsername, StringComparison.Ordinal) ||
            !CryptographicOperations.FixedTimeEquals(
                SHA256.HashData(Encoding.UTF8.GetBytes(password)),
                SHA256.HashData(Encoding.UTF8.GetBytes(expectedPassword))))
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var expirationMinutes = configuration.GetValue("Jwt:ExpirationMinutes", 60);
        if (expirationMinutes is < 1 or > 1440)
        {
            throw new InvalidOperationException("Jwt:ExpirationMinutes must be between 1 and 1440.");
        }

        var issuer = configuration["Jwt:Issuer"] ?? "CharacterHub";
        var audience = configuration["Jwt:Audience"] ?? "CharacterHubClient";
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer,
            audience,
            [new Claim(ClaimTypes.Name, username)],
            now,
            now.AddMinutes(expirationMinutes),
            credentials);

        return new LoginResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            "Bearer",
            token.ValidTo);
    }
}
