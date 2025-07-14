using LightNap.Core.Configuration;
using LightNap.Core.Data.Entities;
using LightNap.Core.Extensions;
using LightNap.Core.Identity.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace LightNap.Core.Services
{
    /// <summary>
    /// Service for generating JWT access tokens and refresh tokens.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SigningCredentials _signingCredentials;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationMinutes;
        public int ExpirationMinutes => this._expirationMinutes;
        private readonly JsonWebTokenHandler _tokenHandler;

        /// </summary>
        /// <param name="configuration">The configuration settings.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="roleManager">The role manager.</param>
        public TokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this._roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            this._userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

            var tokenKey = this._configuration.GetRequiredSetting("Jwt:Key");
            if (tokenKey.Length < 32) { throw new ArgumentException("The provided setting 'Jwt:Key' must be at least 32 characters long"); }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
            this._signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            this._issuer = this._configuration.GetRequiredSetting("Jwt:Issuer");
            this._audience = this._configuration.GetRequiredSetting("Jwt:Audience");
            this._expirationMinutes = int.Parse(this._configuration.GetRequiredSetting("Jwt:ExpirationMinutes"));
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
                Issuer = this._issuer,
                Audience = this._audience,
                Claims = claims,
                Expires = DateTime.UtcNow.AddMinutes(this._expirationMinutes),
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
