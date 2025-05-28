using EruMobil.Application.Interfaces.Tokens;
using EruMobil.Domain.Entities;
using Microsoft.AspNetCore.Identity;
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

namespace EruMobil.Infrastructure.Tokens
{
    public class TokenService : ITokenService
    {
        private readonly TokenSettings tokenSettings;
        private readonly UserManager<User> userManager;

        public TokenService(IOptions<TokenSettings> options, UserManager<User> userManager)
        {
            tokenSettings = options.Value;
            this.userManager = userManager;
        }
        public string CreateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<JwtSecurityToken> CreateToken(User user, IList<string> roles, string userAgent)
        {
            var deviceDetector = new DeviceDetectorNET.DeviceDetector(userAgent);
            deviceDetector.Parse();

            var deviceType = deviceDetector.GetDeviceName(); // örn: smartphone, desktop
            var os = deviceDetector.GetOs()?.Match?.Name;   // örn: Android, Windows
            var client = deviceDetector.GetClient()?.Match?.Name; // örn: Chrome, Firefox

            var claims = new List<Claim> {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                    new Claim("device_type", deviceType ?? "unknown"),
                    new Claim("os", os ?? "unknown"),
                    new Claim("browser", client ?? "unknown")
            };
    

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Secret));

            var token = new JwtSecurityToken(
                issuer: tokenSettings.Issuer,
                audience: tokenSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(tokenSettings.TokenValidityInMinutes).AddHours(3),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return token;
        }



        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = false, // Do not validate the token's lifetime
                ValidateIssuerSigningKey = true,
                //ValidIssuer = tokenSettings.Issuer,
                //ValidAudience = tokenSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Secret))
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
