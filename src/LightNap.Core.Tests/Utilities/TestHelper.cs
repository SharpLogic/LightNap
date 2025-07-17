using LightNap.Core.Data.Entities;
using LightNap.Core.Extensions;
using LightNap.Core.Identity.Dto.Request;
using Microsoft.AspNetCore.Identity;

namespace LightNap.Core.Tests.Utilities
{
    /// <summary>
    /// Provides helper methods for creating test users and roles.
    /// </summary>
    internal static class TestHelper
    {
        /// <summary>
        /// Creates a test user asynchronously.
        /// </summary>
        /// <param name="userManager">The user manager to use for creating the user.</param>
        /// <param name="userId">The ID of the user to create.</param>
        /// <param name="userName">The optional user name. If not provided, a random userName will be used.</param>
        /// <param name="email">The optional email. If not provided, a random email will be used.</param>
        /// <param name="email">The optional password. If not provided, a random password will be used.</param>
        /// <returns>The created <see cref="ApplicationUser"/>.</returns>
        public static async Task<ApplicationUser> CreateTestUserAsync(UserManager<ApplicationUser> userManager, string userId, string? userName = null, string? email = null)
        {
            var registerRequestDto = new RegisterRequestDto()
            {
                UserName = userName ?? Guid.NewGuid().ToString("N"),
                Email = email ?? $"{Guid.NewGuid():N}@test.com",
                Password = "P@ssw0rd",
                ConfirmPassword = "P@ssw0rd",
                DeviceDetails = "Test Environment"
            };
            ApplicationUser user = registerRequestDto.ToCreate(false);
            user.Id = userId;
            await userManager.CreateAsync(user);
            return user;
        }

        /// <summary>
        /// Creates a test role asynchronously.
        /// </summary>
        /// <param name="roleManager">The role manager to use for creating the role.</param>
        /// <param name="roleName">The name of the role to create.</param>
        /// <returns>The created <see cref="ApplicationRole"/>.</returns>
        public static async Task<ApplicationRole> CreateTestRoleAsync(RoleManager<ApplicationRole> roleManager, string roleName)
        {
            ApplicationRole role = new(roleName, roleName, roleName);
            await roleManager.CreateAsync(role);
            return role;
        }
    }
}
