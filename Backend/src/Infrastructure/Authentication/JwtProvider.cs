using Application.Abstractions.Auth;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Authentication
{
    internal class JwtProvider : IJwtProvider
    {
        private readonly JwtOptions _jwtOptions;

        public JwtProvider(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public string GenerateToken(User user)
        {
            ValidateSettings();
            var claims = GetUserClaims(user);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _jwtOptions.SingingKey));
            var signingCredentials = new SigningCredentials(key
                                        , SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_jwtOptions.Issuer
                , _jwtOptions.Audience
                , claims
                , null
                , DateTime.UtcNow.AddMinutes(_jwtOptions.LifeTime)
                , signingCredentials);
            var tokenValue= new JwtSecurityTokenHandler().WriteToken(token);

            return tokenValue;
        }
        private void ValidateSettings()
        {
            if (string.IsNullOrWhiteSpace(_jwtOptions.SingingKey))
                throw new InvalidOperationException("JWT Secret is not configured");

            if (_jwtOptions.SingingKey.Length < 32)
                throw new InvalidOperationException("JWT Secret must be at least 32 characters long");

            if (string.IsNullOrWhiteSpace(_jwtOptions.Issuer))
                throw new InvalidOperationException("JWT Issuer is not configured");

            if (string.IsNullOrWhiteSpace(_jwtOptions.Audience))
                throw new InvalidOperationException("JWT Audience is not configured");

            if (_jwtOptions.LifeTime <= 0)
                throw new InvalidOperationException("JWT Lifetime must be greater than 0");
        }

        public DateTimeOffset GetTokenExpiration()
        {
            return DateTimeOffset.UtcNow.AddMinutes(_jwtOptions.LifeTime);
        }
        private List<Claim> GetUserClaims(User user)
        {
            var claims = new List<Claim>
            {
            new (ClaimTypes.NameIdentifier, user.Id.ToString()),
            new (ClaimTypes.Name, user.UserName),
            new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new (JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
            };

            return claims;
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }
    }
}
