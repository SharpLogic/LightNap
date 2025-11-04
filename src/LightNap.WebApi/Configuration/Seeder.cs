using LightNap.Core.Configuration;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Extensions;
using LightNap.Core.Identity.Dto.Request;
using LightNap.Core.StaticContents.Dto.Request;
using LightNap.Core.StaticContents.Enums;
using LightNap.Core.StaticContents.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Data;
using System.Text.RegularExpressions;

namespace LightNap.WebApi.Configuration
{
    /// <summary>
    /// Provides functionality to seed roles, users, and application-specific content into the database. This class is
    /// designed to be used during application startup to ensure that required data is present.
    /// </summary>
    /// <remarks>The <see cref="Seeder"/> class is responsible for seeding roles, users, and other application
    /// content into the database. It supports both baseline seeding (e.g., roles and administrators) and
    /// environment-specific seeding through the use of a partial method. To customize environment-specific seeding,
    /// implement the <see cref="SeedEnvironmentContent"/> method in a partial class (e.g., Seeder.Development.cs). 
    /// This class relies on several dependencies, including <see cref="RoleManager{T}"/>, <see cref="UserManager{T}"/>,
    /// and <see cref="ApplicationDbContext"/>, to perform its operations. It also uses configuration options to
    /// determine the users and roles to seed.</remarks>
    /// <param name="serviceProvider">A service provider to pull in dependencies from environment-specific classes like Seeder.Development.cs.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="roleManager">The role manager.</param>
    /// <param name="userManager">The user manager.</param>
    /// <param name="contentService">The static content service.</param>
    /// <param name="seededUserConfigurations">The users to seed.</param>
    /// <param name="applicationSettings">The configured application settings.</param>
    public partial class Seeder(
        IServiceProvider serviceProvider,
        ILogger<Seeder> logger,
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        IStaticContentService contentService,
        IOptions<Dictionary<string, List<SeededUserConfiguration>>> seededUserConfigurations,
        IOptions<ApplicationSettings> applicationSettings)
    {

        /// <summary>
        /// Run seeding functionality necessary every time an application loads, regardless of environment.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SeedAsync()
        {
            await this.SeedRolesAsync();
            await this.SeedUsersAsync();
            await this.SeedStaticContentAsync();
            await this.SeedApplicationContentAsync();
            await this.SeedEnvironmentContentAsync();
        }

        private async Task SeedStaticContentAsync()
        {
            string basePath = Path.Combine(AppContext.BaseDirectory, "StaticContent");
            if (!Directory.Exists(basePath))
            {
                logger.LogWarning("Static content directory not found: '{basePath}'", basePath);
                return;
            }

            // Track keys during this scan to detect duplicates in the file system
            var keyRegistry = new Dictionary<string, (StaticContentType Type, StaticContentReadAccess ReadAccess, string Path)>();

            // Pattern: {languageCode}.{extension}
            Regex fileNameRegex = new Regex(@"^([a-z]{2})\.([a-z0-9]+)$", RegexOptions.Compiled);

            // Iterate through type folders (zones, pages, others added in future)
            foreach (var typeDir in Directory.GetDirectories(basePath))
            {
                string typeName = Path.GetFileName(typeDir);
                StaticContentType type = typeName.ToLowerInvariant() switch
                {
                    "zones" => StaticContentType.Zone,
                    "pages" => StaticContentType.Page,
                    _ => throw new InvalidOperationException($"Invalid static content type directory: '{typeDir}'")
                };

                // Iterate through read access folders (public, authenticated, explicit)
                foreach (var accessDir in Directory.GetDirectories(typeDir))
                {
                    string accessName = Path.GetFileName(accessDir);
                    StaticContentReadAccess readAccess = accessName.ToLowerInvariant() switch
                    {
                        "public" => StaticContentReadAccess.Public,
                        "authenticated" => StaticContentReadAccess.Authenticated,
                        "explicit" => StaticContentReadAccess.Explicit,
                        _ => throw new InvalidOperationException($"Invalid static content read access directory: '{accessDir}'")
                    };

                    // Iterate through key folders
                    foreach (var keyDir in Directory.GetDirectories(accessDir))
                    {
                        string key = Path.GetFileName(keyDir);

                        // Validate key format (kebab-case)
                        if (!Regex.IsMatch(key, @"^[a-z0-9]+(-[a-z0-9]+)*$"))
                        {
                            throw new InvalidOperationException($"Invalid static content key directory name: '{key}'. Must be kebab-case.");
                        }

                        // Check for duplicate keys in the file system
                        if (keyRegistry.TryGetValue(key, out var existing))
                        {
                            throw new InvalidOperationException(
                                $"Duplicate static content key '{key}' found in file system.\n" +
                                $"  First location: {existing.Path} (Type: {existing.Type}, Access: {existing.ReadAccess})\n" +
                                $"  Second location: {keyDir} (Type: {type}, Access: {readAccess})\n" +
                                $"Each key must appear in exactly one location.");
                        }

                        // Register this key
                        keyRegistry[key] = (type, readAccess, keyDir);

                        // Iterate through language files
                        foreach (var filePath in Directory.GetFiles(keyDir))
                        {
                            string fileName = Path.GetFileName(filePath);
                            Match match = fileNameRegex.Match(fileName);

                            if (!match.Success)
                            {
                                logger.LogWarning("Skipping invalid static content file: '{filePath}'. Expected format: {{languageCode}}.{{extension}}", filePath);
                                continue;
                            }

                            string languageCode = match.Groups[1].Value;
                            string extension = match.Groups[2].Value;

                            StaticContentFormat format = extension.ToLowerInvariant() switch
                            {
                                "html" => StaticContentFormat.Html,
                                "md" => StaticContentFormat.Markdown,
                                "txt" => StaticContentFormat.PlainText,
                                _ => throw new InvalidOperationException($"Invalid static content format in file: '{filePath}'")
                            };

                            string content = await File.ReadAllTextAsync(filePath);

                            await this.SeedStaticContentLanguageAsync(key, type, readAccess, languageCode, format, content);
                        }
                    }
                }
            }

            logger.LogInformation("Seeded {count} static content keys from file system", keyRegistry.Count);
        }

        private async Task SeedStaticContentLanguageAsync(string key, StaticContentType type, StaticContentReadAccess readAccess,
            string languageCode, StaticContentFormat format, string content)
        {
            var staticContent = await contentService.GetStaticContentAsync(key);
            if (staticContent is null)
            {
                staticContent = await contentService.CreateStaticContentAsync(
                    new CreateStaticContentDto()
                    {
                        Key = key,
                        Type = type,
                        Status = StaticContentStatus.Published,
                        ReadAccess = readAccess
                    });

                logger.LogInformation("Created static content with key '{key}'", key);
            }

            var existingLanguage = await contentService.GetStaticContentLanguageAsync(key, languageCode);
            if (existingLanguage is null)
            {
                await contentService.CreateStaticContentLanguageAsync(
                    staticContent.Key,
                    languageCode,
                    new CreateStaticContentLanguageDto()
                    {
                        Content = content,
                        Format = format,
                    });

                logger.LogInformation("Created static content language '{languageCode}' for key '{key}'", languageCode, key);
            }
        }

        /// <summary>
        /// Seeds the roles in the application.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SeedRolesAsync()
        {
            foreach (ApplicationRole role in ApplicationRoles.All)
            {
                if (!await roleManager.RoleExistsAsync(role.Name!))
                {
                    var result = await roleManager.CreateAsync(role);
                    if (!result.Succeeded)
                    {
                        throw new ArgumentException($"Unable to create role '{role.Name}': {string.Join("; ", result.Errors.Select(error => error.Description))}");
                    }
                    logger.LogInformation("Added role '{roleName}'", role.Name);
                }
            }

            var roleSet = new HashSet<string>(ApplicationRoles.All.Select(role => role.Name!), StringComparer.OrdinalIgnoreCase);

            foreach (var role in roleManager.Roles.Where(role => role.Name != null && !roleSet.Contains(role.Name)))
            {
                var result = await roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    throw new ArgumentException($"Unable to remove role '{role.Name}': {string.Join("; ", result.Errors.Select(error => error.Description))}");
                }
                logger.LogInformation("Removed role '{roleName}'", role.Name);
            }
        }

        /// <summary>
        /// Seeds the users in the application and adds them to their respective roles.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SeedUsersAsync()
        {
            if (seededUserConfigurations.Value is null) { return; }

            // Loop through the dictionary keys (roles) and add/get each user and add them to the role. Note that we sort the roles alphabetically,
            // so the "earliest" alphabetic instance of a new user will use that email/password.
            foreach (var roleToUsers in seededUserConfigurations.Value.OrderBy(roleToUser => roleToUser.Key)
                .Select(roleToUser => new { Role = roleToUser.Key, Users = roleToUser.Value }))
            {
                if (!string.IsNullOrWhiteSpace(roleToUsers.Role))
                {
                    if (!await roleManager.RoleExistsAsync(roleToUsers.Role)) { throw new ArgumentException($"Unable to find role '{roleToUsers.Role}' to seed users."); }
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
            ApplicationUser? user = await userManager.FindByEmailAsync(email);

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

                user = registerRequestDto.ToCreate(applicationSettings.Value.RequireTwoFactorForNewUsers);

                var result = await userManager.CreateAsync(user, passwordToSet);
                if (!result.Succeeded)
                {
                    throw new ArgumentException($"Unable to create user '{userName}' ('{email}'): {string.Join("; ", result.Errors.Select(error => error.Description))}");
                }

                logger.LogInformation("Created user '{userName}' ('{email}')", userName, email);
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
            if (await userManager.IsInRoleAsync(user, role)) { return; }

            var result = await userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                throw new ArgumentException(
                    $"Unable to add user '{user.UserName}' ('{user.Email}') to role '{role}': {string.Join("; ", result.Errors.Select(error => error.Description))}");
            }
            logger.LogInformation("Added user '{userName}' ('{email}') to role '{roleName}'", user.UserName, user.Email, role);
        }

        /// <summary>
        /// Seeds content in the application. This method runs after baseline seeding (like roles and administrators) and provides an opportunity to
        /// seed any content required to be loaded regardless of environment.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
#pragma warning disable CA1822 // Mark members as static. This needs to be here for the partial method to work properly.
        private Task SeedApplicationContentAsync()
#pragma warning restore CA1822 // Mark members as static
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
