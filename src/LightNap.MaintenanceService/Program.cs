using LightNap.Configuration.Database;
using LightNap.Configuration.DataProtection;
using LightNap.Configuration.Extensions;
using LightNap.Core.Api;
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

using var loggerFactory = LoggerFactory.Create(b => b.AddConsole());
var bootstrapLogger = loggerFactory.CreateLogger("Startup");

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        var env = context.HostingEnvironment;
        config
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddLogging(configure => configure.AddConsole());

        var databaseSettings = context.Configuration.GetRequiredSection<DatabaseSettings>("Database");
        var dataProtectionSettings = context.Configuration.GetRequiredSection<DataProtectionSettings>("DataProtection");

        services.AddLightNapDataProtectionServices(dataProtectionSettings, logger: bootstrapLogger);

        switch (databaseSettings.Provider)
        {
            case DatabaseProvider.InMemory:
                Trace.TraceWarning($"The MaintenanceService is configured to use the '{databaseSettings.Provider}' database provider, so there won't be any DB data");
                services.AddLightNapInMemoryDatabase(logger: bootstrapLogger);
                break;
            case DatabaseProvider.Sqlite:
                services.AddLightNapSqlite(context.Configuration.GetRequiredConnectionString("DefaultConnection"), bootstrapLogger);
                break;
            case DatabaseProvider.SqlServer:
                services.AddLightNapSqlServer(context.Configuration.GetRequiredConnectionString("DefaultConnection"), bootstrapLogger);
                break;
            default: throw new ArgumentException($"Unsupported 'DatabaseProvider' setting: '{databaseSettings.Provider}'");
        }

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
