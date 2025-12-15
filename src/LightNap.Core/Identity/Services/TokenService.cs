using LightNap.Core.Configuration.Authentication;
using LightNap.Core.Configuration.Authorization;
using LightNap.Core.Data.Entities;
using LightNap.Core.Identity.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;

namespace LightNap.Core.Services
{
    /// <summary>
    /// Service for generating JWT access tokens and refresh tokens.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SigningCredentials _signingCredentials;
        private readonly JsonWebTokenHandler _tokenHandler;
        private readonly JwtSettings _jwtSettings;

        /// <summary>
        /// The number of minutes configured for access token expiration.
        /// </summary>
        public int ExpirationMinutes => this._jwtSettings.ExpirationMinutes;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenService"/> class.
        /// </summary>
        /// <param name="jwtSettings">JWT settings to use.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="roleManager">The role manager.</param>
        public TokenService(IOptions<JwtSettings> jwtSettings, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            ArgumentNullException.ThrowIfNull(jwtSettings, nameof(jwtSettings));
            Validator.ValidateObject(jwtSettings.Value, new ValidationContext(jwtSettings.Value), true);
            this._jwtSettings = jwtSettings.Value;
            this._roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            this._userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._jwtSettings.Key));
            this._signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            this._tokenHandler = new JsonWebTokenHandler();
        }

        /// <summary>
        /// Generates an access token for the specified user.
        /// </summary>
        /// <param name="user">The user for whom to generate the token.</param>
        /// <returns>The generated access token.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the user parameter is null.</exception>
        public async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var claims = new Dictionary<string, object>
            {
                [ClaimTypes.NameIdentifier] = user.Id,
                [ClaimTypes.Email] = user.Email!,
                [ClaimTypes.Name] = user.UserName!,
            };

            List<Claim> customClaims = [];

            IList<string> roles = await this._userManager.GetRolesAsync(user);
            if (roles.Any())
            {
                claims.Add(ClaimTypes.Role, roles);

                foreach (var role in roles)
                {
                    var roleClaims = await this._roleManager.GetClaimsAsync(ApplicationRoles.All.First(r => r.Name == role));
                    customClaims.AddRange(roleClaims);
                }
            }

            customClaims.AddRange(await this._userManager.GetClaimsAsync(user));

            foreach (var claimGroup in customClaims.GroupBy(c => c.Type))
            {
                claims.Add(claimGroup.Key, claimGroup.Select(c => c.Value).Distinct().ToArray());
            }

            var token = new SecurityTokenDescriptor()
            {
                Issuer = this._jwtSettings.Issuer,
                Audience = this._jwtSettings.Audience,
                Claims = claims,
                Expires = DateTime.UtcNow.AddMinutes(this._jwtSettings.ExpirationMinutes),
                SigningCredentials = this._signingCredentials
            };

            return this._tokenHandler.CreateToken(token);
        }

        /// <summary>
        /// Generates a refresh token.
        /// </summary>
        /// <returns>The generated refresh token.</returns>
        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
