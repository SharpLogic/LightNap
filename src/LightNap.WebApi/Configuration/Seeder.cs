using LightNap.Core.Configuration;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Extensions;
using LightNap.Core.Identity.Dto.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Data;

namespace LightNap.WebApi.Configuration
{
    /// <summary>
    /// Class responsible for seeding content in the application upon load.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="Seeder"/> class.
    /// </remarks>
    /// <param name="serviceProvider">Service provider to pull dependencies from.</param>
    public partial class Seeder(IServiceProvider serviceProvider)
    {
        private readonly RoleManager<ApplicationRole> _roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        private readonly ILogger<Seeder> _logger = serviceProvider.GetRequiredService<ILogger<Seeder>>();
        private readonly UserManager<ApplicationUser> _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        private readonly ApplicationDbContext _db = serviceProvider.GetRequiredService<ApplicationDbContext>();
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly IOptions<Dictionary<string, List<SeededUserConfiguration>>> _seededUserConfigurations = serviceProvider.GetRequiredService<IOptions<Dictionary<string, List<SeededUserConfiguration>>>>();
        private readonly IOptions<ApplicationSettings> _applicationSettings = serviceProvider.GetRequiredService<IOptions<ApplicationSettings>>();

        /// <summary>
        /// Run seeding functionality necessary every time an application loads, regardless of environment.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SeedAsync()
        {
            await this.SeedRolesAsync();
            await this.SeedUsersAsync();
            await this.SeedApplicationContentAsync();
            await this.SeedEnvironmentContentAsync();
        }

        /// <summary>
        /// Seeds the roles in the application.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SeedRolesAsync()
        {
            foreach (ApplicationRole role in ApplicationRoles.All)
            {
                if (!await this._roleManager.RoleExistsAsync(role.Name!))
                {
                    var result = await this._roleManager.CreateAsync(role);
                    if (!result.Succeeded)
                    {
                        throw new ArgumentException($"Unable to create role '{role.Name}': {string.Join("; ", result.Errors.Select(error => error.Description))}");
                    }
                    this._logger.LogInformation("Added role '{roleName}'", role.Name);
                }
            }

            var roleSet = new HashSet<string>(ApplicationRoles.All.Select(role => role.Name!), StringComparer.OrdinalIgnoreCase);

            foreach (var role in this._roleManager.Roles.Where(role => role.Name != null && !roleSet.Contains(role.Name)))
            {
                var result = await this._roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    throw new ArgumentException($"Unable to remove role '{role.Name}': {string.Join("; ", result.Errors.Select(error => error.Description))}");
                }
                this._logger.LogInformation("Removed role '{roleName}'", role.Name);
            }
        }

        /// <summary>
        /// Seeds the users in the application and adds them to their respective roles.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SeedUsersAsync()
        {
            if (this._seededUserConfigurations.Value is null) { return; }

            // Loop through the dictionary keys (roles) and add/get each user and add them to the role. Note that we sort the roles alphabetically,
            // so the "earliest" alphabetic instance of a new user will use that email/password.
            foreach (var roleToUsers in this._seededUserConfigurations.Value.OrderBy(roleToUser => roleToUser.Key)
                .Select(roleToUser => new { Role = roleToUser.Key, Users = roleToUser.Value }))
            {
                if (!string.IsNullOrWhiteSpace(roleToUsers.Role))
                {
                    if (!await this._roleManager.RoleExistsAsync(roleToUsers.Role)) { throw new ArgumentException($"Unable to find role '{roleToUsers.Role}' to seed users."); }
                }

                foreach (var seededUser in roleToUsers.Users)
                {
                    ApplicationUser user = await this.GetOrCreateUserAsync(seededUser.UserName, seededUser.Email, seededUser.Password);

                    if (!string.IsNullOrWhiteSpace(roleToUsers.Role))
                    {
                        await this.AddUserToRole(user, roleToUsers.Role);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new user in the application.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="email">The email address.</param>
        /// <param name="password">The password.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task<ApplicationUser> GetOrCreateUserAsync(string userName, string email, string? password = null)
        {
            ApplicationUser? user = await this._userManager.FindByEmailAsync(email);

            if (user is null)
            {
                bool passwordProvided = !string.IsNullOrWhiteSpace(password);
                string passwordToSet = passwordProvided ? password! : $"P@ssw0rd{Guid.NewGuid()}";

                var registerRequestDto = new RegisterRequestDto()
                {
                    ConfirmPassword = passwordToSet,
                    DeviceDetails = "Seeder",
                    Email = email,
                    Password = passwordToSet,
                    UserName = userName
                };

                user = registerRequestDto.ToCreate(this._applicationSettings.Value.RequireTwoFactorForNewUsers);

                var result = await this._userManager.CreateAsync(user, passwordToSet);
                if (!result.Succeeded)
                {
                    throw new ArgumentException($"Unable to create user '{userName}' ('{email}'): {string.Join("; ", result.Errors.Select(error => error.Description))}");
                }

                this._logger.LogInformation("Created user '{userName}' ('{email}')", userName, email);
            }

            return user;
        }

        /// <summary>
        /// Adds a user to a specified role if they're not already in it.
        /// </summary>
        /// <param name="user">The user to add to the role.</param>
        /// <param name="role">The role to add the user to.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task AddUserToRole(ApplicationUser user, string role)
        {
            if (await this._userManager.IsInRoleAsync(user, role)) { return; }

            var result = await this._userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                throw new ArgumentException(
                    $"Unable to add user '{user.UserName}' ('{user.Email}') to role '{role}': {string.Join("; ", result.Errors.Select(error => error.Description))}");
            }
            this._logger.LogInformation("Added user '{userName}' ('{email}') to role '{roleName}'", user.UserName, user.Email, role);
        }

        /// <summary>
        /// Seeds content in the application. This method runs after baseline seeding (like roles and administrators) and provides an opportunity to
        /// seed any content required to be loaded regardless of environment.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private Task SeedApplicationContentAsync()
        {
            // TODO: Add any seeding code you want run every time the app loads in any environment. For environment-specific seeding, see SeedEnvironmentContent().

            return Task.CompletedTask;
        }

        /// <summary>
        /// Seeds content in the application based on the implementation of a SeedEnvironmentContent partial method in the class. To use this, add a Seeder 
        /// partial class (like Seeder.Development.cs) that implements the private method SeedEnvironmentContent(). It runs after SeedApplicationContentAsync()
        /// and is always executed on load if it exists.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task SeedEnvironmentContentAsync()
        {
            this.SeedEnvironmentContent();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Optional partial to implement in a new class (like Seeder.Development.cs) to seed environment-specific content.
        /// </summary>
        partial void SeedEnvironmentContent();
    }
}
