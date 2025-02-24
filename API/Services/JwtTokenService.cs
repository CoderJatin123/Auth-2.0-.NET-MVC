using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace API.JwtTokenService;

public class JwtTokenService
{
    private readonly IConfiguration _configuration;
    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public (string AccessToken, string RefreshToken, DateTime RefreshExpiry)
        GenerateTokens(string userId)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("Userid", userId),
        };
        var key = new
            SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var accessToken = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires:
            DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:AccessTokenExpiryMinutes"])),
            signingCredentials: signIn
        );
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        DateTime refreshExpiry =
            DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:RefreshTokenExpiryMinutes"]));
        return (new JwtSecurityTokenHandler().WriteToken(accessToken), refreshToken,
            refreshExpiry);
    }
}