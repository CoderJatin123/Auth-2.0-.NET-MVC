using System.Collections.Concurrent;
using API.JwtTokenService;
using Microsoft.AspNetCore.Mvc;
using Repositories.Models;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private static ConcurrentDictionary<string, (string Username, DateTime Expiry)> _refreshTokens = new();
        private readonly JwtTokenService _jwtTokenService;
        public AuthApiController(JwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }
        [HttpPost("token")]
        public IActionResult GenerateToken([FromForm] LoginRequest request)
        {
            if (request.Username != "admin" || request.Password != "password")
                return Unauthorized("Invalid credentials");
            var tokens = _jwtTokenService.GenerateTokens(request.Username);
            _refreshTokens[tokens.RefreshToken] = (request.Username, tokens.RefreshExpiry);
            return Ok(new { accessToken = tokens.AccessToken, refreshToken = tokens.RefreshToken });
        }
        
        [HttpPost("refresh")]
        public IActionResult RefreshToken([FromForm] RefreshRequest request)
        {
            if (!_refreshTokens.TryGetValue(request.RefreshToken, out var tokenInfo))
                return Unauthorized("Invalid refresh token");
            if (DateTime.UtcNow > tokenInfo.Expiry) // Check if refresh token is expired
            {
                _refreshTokens.TryRemove(request.RefreshToken, out _); // Remove expired token
                return Unauthorized("Refresh token expired");
            }
            var tokens = _jwtTokenService.GenerateTokens(tokenInfo.Username);
            _refreshTokens[tokens.RefreshToken] = (tokenInfo.Username, tokens.RefreshExpiry);
            return Ok(new { accessToken = tokens.AccessToken, refreshToken = tokens.RefreshToken });
        }
    }
}
