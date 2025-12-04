using LightNap.Core.Api;
using LightNap.Core.Configuration.Authentication;
using LightNap.Core.Configuration.Authorization;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Email.Interfaces;
using LightNap.Core.Extensions;
using LightNap.Core.Identity.Dto.Request;
using LightNap.Core.Identity.Dto.Response;
using LightNap.Core.Identity.Interfaces;
using LightNap.Core.Identity.Models;
using LightNap.Core.Interfaces;
using LightNap.Core.Notifications.Dto.Request;
using LightNap.Core.Notifications.Enums;
using LightNap.Core.Notifications.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace LightNap.Core.Identity.Services;

/// <summary>
/// Service for managing identity.
/// </summary>
public class ExternalLoginService(ILogger<ExternalLoginService> logger,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ApplicationDbContext db,
    ITokenService tokenService,
    IEmailService emailService,
    INotificationService notificationService,
    IOptions<AuthenticationSettings> authenticationSettings,
    IUserContext userContext,
    ICookieManager cookieManager,
    IRefreshTokenService refreshTokenService,
    HybridCache cache) : IExternalLoginService
{
    private readonly InternalLoginService _internalLoginService = new(userManager, tokenService, emailService, authenticationSettings, cookieManager, refreshTokenService);

    /// <inheritdoc />
    public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl)
    {
        return signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
    }

    /// <inheritdoc />
    public async Task<string> ExternalLoginCallbackAsync()
    {
        var info = await signInManager.GetExternalLoginInfoAsync() ?? throw new UserFriendlyApiException("Unable to link your external login info");

        // Generate a temporary token to store OAuth info. This is the token we'll give the frontend to complete the registration.
        var confirmationToken = Guid.NewGuid().ToString();
        // Store OAuth info temporarily. This needs to be available long enough for the user to provide supplemental
        // registration info (like required profile fields) and accept terms.
        await cache.SetAsync(confirmationToken, new PendingExternalUserLoginInfo(info),
            new HybridCacheEntryOptions() { Expiration = TimeSpan.FromMinutes(10) });

        return confirmationToken;
    }

    /// <inheritdoc />
    public async Task<ExternalLoginSuccessDto> GetExternalLoginResultAsync(string confirmationToken)
    {
        var (_, value) = await cache.TryGetValueAsync<PendingExternalUserLoginInfo>(confirmationToken);
        var info = value ?? throw new UserFriendlyApiException("OAuth session expired. Please try again.");

        var externalLoginUser = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

        if (userContext.IsAuthenticated)
        {
            await cache.RemoveAsync(confirmationToken);

            var loggedInUser = await userManager.FindByIdAsync(userContext.GetUserId()) ?? throw new UserFriendlyApiException("Unable to link external login to your account.");

            if (externalLoginUser is not null)
            {
                if (externalLoginUser.Id != loggedInUser.Id)
                {
                    throw new UserFriendlyApiException(
                        "This external login is already linked to an account. Log into that account to remove it there " +
                        "if you want to associate it with this new account instead.");
                }

                throw new UserFriendlyApiException("This external login is already linked to your account.");
            }

            var result = await userManager.AddLoginAsync(loggedInUser, info);
            if (!result.Succeeded)
            {
                throw new UserFriendlyApiException(result.Errors.Select(e => e.Description));
            }

            return new ExternalLoginSuccessDto() { Type = ExternalLoginSuccessType.NewAccountLink };
        }

        if (externalLoginUser is not null)
        {
            return new ExternalLoginSuccessDto() { Type = ExternalLoginSuccessType.AlreadyLinked };
        }

        return new ExternalLoginSuccessDto()
        {
            Type = ExternalLoginSuccessType.RequiresRegistration,
            Email = info.Email,
            UserName = info.UserName,
        };
    }

    /// <inheritdoc />
    public async Task<LoginSuccessDto> CompleteExternalLoginAsync(string confirmationToken, ExternalLoginRequestDto loginRequestDto)
    {
        ArgumentNullException.ThrowIfNull(loginRequestDto);
        Validator.ValidateObject(loginRequestDto, new ValidationContext(loginRequestDto), true);

        if (userContext.IsAuthenticated) { throw new UserFriendlyApiException("You are already logged in."); }

        var (_, value) = await cache.TryGetValueAsync<PendingExternalUserLoginInfo>(confirmationToken);
        var info = value ?? throw new UserFriendlyApiException("OAuth session expired. Please try again.");
        await cache.RemoveAsync(confirmationToken);

        var user = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey) ?? throw new UserFriendlyApiException("The user associated with this request does not exist.");

        // Log the user in.
        return await this._internalLoginService.HandleUserLoginAsync(user, loginRequestDto.RememberMe, loginRequestDto.DeviceDetails);
    }

    /// <inheritdoc />
    public async Task<LoginSuccessDto> CompleteExternalLoginRegistrationAsync(string confirmationToken, ExternalLoginRegisterRequestDto registerRequestDto)
    {
        ArgumentNullException.ThrowIfNull(registerRequestDto);
        Validator.ValidateObject(registerRequestDto, new ValidationContext(registerRequestDto), true);

        if (userContext.IsAuthenticated) { throw new UserFriendlyApiException("You are already logged in."); }

        var (_, value) = await cache.TryGetValueAsync<PendingExternalUserLoginInfo>(confirmationToken);
        var info = value ?? throw new UserFriendlyApiException("OAuth session expired. Please try again.");

        // Make sure this external login is not already associated with another account.
        if (await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey) is not null)
        {
            throw new UserFriendlyApiException(
                "This external login is already linked to an account. Log into that account to remove it there " +
                "if you want to associate it with this new account instead.");
        }

        // Make sure the requested email does not already exist.
        var user = await userManager.FindByEmailAsync(registerRequestDto.Email);
        if (user is not null)
        {
            throw new UserFriendlyApiException(
                "A user with this email already exists. Log in with that account before linking an external login.");
        }

        // Check for username conflict before creating the user.
        if (await userManager.FindByNameAsync(registerRequestDto.UserName) != null)
        {
            throw new UserFriendlyApiException("Username is already taken.");
        }

        // Create the new user
        user = registerRequestDto.ToCreate(authenticationSettings.Value.RequireTwoFactorForNewUsers);
        var result = await userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            throw new UserFriendlyApiException(result.Errors.Select(e => e.Description));
        }

        // Associate the external login.
        result = await userManager.AddLoginAsync(user, info);
        if (!result.Succeeded)
        {
            throw new UserFriendlyApiException(result.Errors.Select(e => e.Description));
        }

        // Clean up temporary data and send out notifications/logging.
        await cache.RemoveAsync(confirmationToken);
        await notificationService.CreateSystemNotificationForRoleAsync(
            ApplicationRoles.Administrator.Name!,
            new CreateNotificationRequestDto()
            {
                Type = NotificationType.AdministratorNewUserRegistration,
                Data = new Dictionary<string, object>() { { "userId", user.Id } }
            });
        logger.LogInformation("New user '{userName}' ('{email}') registered via OAuth '{provider}'.", user.UserName, user.Email, info.ProviderDisplayName);

        // Log the user in.
        return await this._internalLoginService.HandleUserLoginAsync(user, registerRequestDto.RememberMe, registerRequestDto.DeviceDetails);
    }

    /// <inheritdoc />
    public async Task<List<ExternalLoginDto>> GetMyExternalLoginsAsync()
    {
        userContext.AssertAuthenticated();
        var user = await userManager.FindByIdAsync(userContext.GetUserId()) ?? throw new UserFriendlyApiException("Unable to get external logins.");

        var logins = await userManager.GetLoginsAsync(user);
        return logins.Select(login => login.ToDto()).ToList();
    }

    /// <inheritdoc />
    public async Task RemoveMyLoginAsync(string loginProvider, string providerKey)
    {
        userContext.AssertAuthenticated();
        var user = await userManager.FindByIdAsync(userContext.GetUserId()) ?? throw new UserFriendlyApiException("Unable to remove external login.");

        var result = await userManager.RemoveLoginAsync(user, loginProvider, providerKey);
        if (!result.Succeeded)
        {
            throw new UserFriendlyApiException(result.Errors.Select(e => e.Description));
        }
    }

    /// <inheritdoc />
    public async Task<PagedResponseDto<AdminExternalLoginDto>> SearchExternalLoginsAsync(SearchExternalLoginsRequestDto searchDto)
    {
        Validator.ValidateObject(searchDto, new ValidationContext(searchDto), true);
        userContext.AssertAdministrator();

        var query = db.UserLogins.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchDto.LoginProvider))
        {
            query = query.Where(login => login.LoginProvider == searchDto.LoginProvider);
        }

        if (!string.IsNullOrWhiteSpace(searchDto.UserId))
        {
            query = query.Where(login => login.UserId == searchDto.UserId);
        }

        int totalCount = await query.CountAsync();

        query = query
            .OrderBy(login => login.LoginProvider)
            .ThenBy(login => login.UserId);

        if (searchDto.PageNumber > 1)
        {
            query = query.Skip((searchDto.PageNumber - 1) * searchDto.PageSize);
        }

        var userLogins = await query
            .Take(searchDto.PageSize)
            .Select(login => login.ToAdminDto())
            .ToListAsync();

        return new PagedResponseDto<AdminExternalLoginDto>(userLogins, searchDto.PageNumber, searchDto.PageSize, totalCount);
    }

    /// <inheritdoc />
    public async Task RemoveExternalLoginAsync(string userId, string loginProvider, string providerKey)
    {
        userContext.AssertAdministrator();
        var user = await userManager.FindByIdAsync(userId) ?? throw new UserFriendlyApiException("Unable to remove external login.");
        var result = await userManager.RemoveLoginAsync(user, loginProvider, providerKey);
        if (!result.Succeeded)
        {
            throw new UserFriendlyApiException(result.Errors.Select(e => e.Description));
        }
    }
}