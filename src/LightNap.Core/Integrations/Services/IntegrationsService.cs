using LightNap.Core.Api;
using LightNap.Core.Configuration.Integrations;
using LightNap.Core.Data;
using LightNap.Core.Extensions;
using LightNap.Core.Integrations.Dto.Request;
using LightNap.Core.Integrations.Dto.Response;
using LightNap.Core.Integrations.Interfaces;
using LightNap.Core.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;

namespace LightNap.Core.Integrations.Services;

/// <summary>  
/// Service for managing user integrations.
/// </summary>  
public class IntegrationsService(ApplicationDbContext db, IUserContext userContext, IDataProtectionProvider dataProtectionProvider) : IIntegrationsService
{
    private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("IntegrationSecretsProtector");

    /// <inheritdoc/>
    public SupportedIntegrationsDto GetSupportedIntegrations()
    {
        return new SupportedIntegrationsDto
        {
            Providers = new ReadOnlyCollection<IntegrationProviderDefinition>(IntegrationsConfig.Providers),
            Categories = new ReadOnlyCollection<IntegrationCategoryDefinition>(IntegrationsConfig.Categories),
            Features = new ReadOnlyCollection<IntegrationFeatureDefinition>(IntegrationsConfig.Features)
        };
    }

    /// <inheritdoc/>
    public async Task<List<IntegrationDto>> GetMyIntegrationsAsync()
    {
        userContext.AssertAuthenticated();
        return await db.Integrations.Where(i => i.UserId == userContext.GetUserId()).Select(i => i.ToDto(this._dataProtector)).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IntegrationDto> CreateMyIntegrationAsync(CreateIntegrationRequestDto createDto)
    {
        userContext.AssertAuthenticated();
        var integration = createDto.ToEntity(userContext.GetUserId(), this._dataProtector);
        db.Integrations.Add(integration);
        await db.SaveChangesAsync();
        return integration.ToDto(this._dataProtector);
    }

    /// <inheritdoc/>
    public async Task<PagedResponseDto<AdminIntegrationDto>> SearchIntegrationsAsync(SearchIntegrationsRequestDto searchDto)
    {
        userContext.AssertAdministrator();

        var query = db.Integrations.AsQueryable();

        if (searchDto.Provider is not null)
        {
            query = query.Where(i => i.Provider == searchDto.Provider);
        }

        if (searchDto.UserId is not null)
        {
            query = query.Where(i => i.UserId == searchDto.UserId);
        }

        query = query.OrderBy(i => i.Provider).ThenBy(i => i.UserId);

        int skip = (searchDto.PageNumber - 1) * searchDto.PageSize;

        int totalCount = await query.CountAsync();
        var items = await query.Skip(skip).Take(searchDto.PageSize).Select(i => i.ToAdminDto(this._dataProtector)).ToListAsync();

        return new PagedResponseDto<AdminIntegrationDto>(items, searchDto.PageNumber, searchDto.PageSize, totalCount);
    }

    /// <inheritdoc/>
    public async Task<IntegrationDto> UpdateMyIntegrationAsync(int integrationId, UpdateIntegrationRequestDto updateDto)
    {
        userContext.AssertAuthenticated();
        var integration = await db.Integrations.FirstOrDefaultAsync(i => i.Id == integrationId && i.UserId == userContext.GetUserId())
            ?? throw new UserFriendlyApiException($"Integration not found.");
        updateDto.UpdateEntity(integration, this._dataProtector);
        await db.SaveChangesAsync();
        return integration.ToDto(this._dataProtector);
    }

    /// <inheritdoc/>
    public async Task DeleteIntegrationAsync(int integrationId)
    {
        userContext.AssertAdministrator();
        var integration = await db.Integrations.FindAsync(integrationId) ?? throw new UserFriendlyApiException($"Integration with ID '{integrationId}' not found.");

        db.Integrations.Remove(integration);
        await db.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteMyIntegrationAsync(int integrationId)
    {
        userContext.AssertAuthenticated();
        var integration = await db.Integrations.FirstOrDefaultAsync(i => i.Id == integrationId && i.UserId == userContext.GetUserId())
             ?? throw new UserFriendlyApiException($"Integration not found.");

        db.Integrations.Remove(integration);
        await db.SaveChangesAsync();
    }
}