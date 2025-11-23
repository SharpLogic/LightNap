using LightNap.Core.Api;
using LightNap.Core.Configuration;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Extensions;
using LightNap.Core.Identity.Services;
using LightNap.Core.Interfaces;
using LightNap.Core.UserSettings.Interfaces;
using LightNap.Core.UserSettings.Services;
using LightNap.DataProviders.Sqlite.Extensions;
using LightNap.DataProviders.SqlServer.Extensions;
using LightNap.MaintenanceService;
using LightNap.MaintenanceService.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddLogging(configure => configure.AddConsole());

        var databaseSettings = context.Configuration.GetRequiredSection<DatabaseSettings>("Database");

        switch (databaseSettings.Provider)
        {
            case DatabaseProvider.InMemory:
                Trace.TraceWarning($"The MaintenanceService is configured to use the '{databaseSettings.Provider}' database provider, so there won't be any DB data");
                services.AddLightNapInMemoryDatabase();
                break;
            case DatabaseProvider.Sqlite:
                services.AddLightNapSqlite(context.Configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentException($"A 'DefaultConnection' connection string is required for '{databaseSettings.Provider}'"));
                break;
            case DatabaseProvider.SqlServer:
                services.AddLightNapSqlServer(context.Configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentException($"A 'DefaultConnection' connection string is required for '{databaseSettings.Provider}'"));
                break;
            default: throw new ArgumentException($"Unsupported 'DatabaseProvider' setting: '{databaseSettings.Provider}'");
        }

        services.AddOptions<AuthenticationSettings>()
            .Bind(context.Configuration.GetRequiredSection("Authentication"))
            .ValidateDataAnnotations();
        services.AddOptions<JwtSettings>()
            .Bind(context.Configuration.GetRequiredSection("Jwt"))
            .ValidateDataAnnotations();

        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IUserContext, SystemUserContext>();
        services.AddScoped<IUserSettingsService, UserSettingsService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        // Manage the tasks to run here. All transient dependencies added for IMaintenanceTask will be in the collection passed to MainService.
        services.AddTransient<IMaintenanceTask, CountUsersMaintenanceTask>();
        services.AddTransient<IMaintenanceTask, PurgeExpiredRefreshTokensMaintenanceTask>();
        services.AddTransient<IMaintenanceTask, PurgeUnusedUserSettingsMaintenanceTask>();

        services.AddTransient<MainService>();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var mainService = services.GetRequiredService<MainService>();
    await mainService.RunAsync();
}