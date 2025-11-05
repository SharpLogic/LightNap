using LightNap.Core.Configuration;
using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Interfaces;

namespace LightNap.Core.Tests.Utilities
{
    /// <summary>
    /// Test implementation of IUserContext for unit testing purposes.
    /// </summary>
    internal class TestUserContext : IUserContext
    {
        /// <inheritdoc />
        public bool IsAdministrator => this.IsInRole(Constants.Roles.Administrator);

        /// <inheritdoc />
        public bool IsAuthenticated => this.UserId is not null;

        /// <summary>
        /// Gets or sets the IP address of the user.
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public string? UserId { get; private set; }

        /// <summary>
        /// The roles.
        /// </summary>
        public IReadOnlyCollection<string> Roles => this._roles.AsReadOnly();
        private List<string> _roles = [];

        /// <summary>
        /// The claims.
        /// </summary>
        public IReadOnlyCollection<ClaimDto> Claims => this._claims.AsReadOnly();
        private List<ClaimDto> _claims = [];

        /// <inheritdoc />
        public string? GetIpAddress()
        {
            return this.IpAddress;
        }

        /// <inheritdoc />
        public string GetUserId()
        {
            if (this.UserId is null) { throw new InvalidOperationException("GetUserId was called without having UserId set first"); }
            return this.UserId;
        }

        /// <inheritdoc />
        public bool IsInRole(string role)
        {
            return this.Roles.Contains(role);
        }

        /// <inheritdoc />
        public bool HasClaim(string type, string value)
        {
            return this.Claims.Any(c => c.Type == type && c.Value == value);
        }

        /// <summary>
        /// Simulates logging out the user.
        /// </summary>
        public void LogOut()
        {
            this.UserId = null;
            this._roles.Clear();
            this._claims.Clear();
        }

        /// <summary>
        /// Logs in a user with no roles or claims.
        /// </summary>
        /// <param name="userId">An optional user ID.</param>
        public void LogIn(string? userId = null)
        {
            this.LogOut();
            this.UserId = userId ?? Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Logs in a user in the Administrator role.
        /// </summary>
        /// <param name="userId">An optional user ID.</param>
        public void LogInAdministrator(string? userId = null)
        {
            this.LogIn(userId);
            this.AddRole(Constants.Roles.Administrator);
        }

        /// <summary>
        /// Logs in a user in the ContentEditor role.
        /// </summary>
        /// <param name="userId">An optional user ID.</param>
        public void LogInContentEditor(string? userId = null)
        {
            this.LogIn(userId);
            this.AddRole(Constants.Roles.ContentEditor);
        }

        /// <summary>
        /// Adds a role to the collection if it does not already exist.
        /// </summary>
        /// <param name="role">The role.</param>
        public void AddRole(string role)
        {
            if (this._roles.Contains(role)) { return; }
            this._roles.Add(role);
        }

        /// <summary>
        /// Removes a role from the collection.
        /// </summary>
        /// <param name="role">The role.</param>
        public void RemoveRole(string role)
        {
            this._roles.Remove(role);
        }

        /// <summary>
        /// Adds a claim with the specified type and value to the collection if it does not already exist.
        /// </summary>
        /// <remarks>If a claim with the same type and value already exists in the collection, the method
        /// does nothing.</remarks>
        /// <param name="type">The type of the claim. This value cannot be null or empty.</param>
        /// <param name="value">The value of the claim. This value cannot be null or empty.</param>
        public void AddClaim(string type, string value)
        {
            var existingClaim = this._claims.FirstOrDefault(c => c.Type == type && c.Value == value);
            if (existingClaim is not null) { return; }

            this._claims.Add(new ClaimDto() { Type = type, Value = value });
        }

        /// <summary>
        /// Removes a claim from the collection based on the specified type and value.
        /// </summary>
        /// <remarks>If multiple claims match the specified type and value, all matching claims will be
        /// removed.</remarks>
        /// <param name="type">The type of the claim to remove. This parameter cannot be null or empty.</param>
        /// <param name="value">The value of the claim to remove. This parameter cannot be null or empty.</param>
        public void RemoveClaim(string type, string value)
        {
            this._claims.RemoveAll(c => c.Type == type && c.Value == value);
        }
    }
}