using LightNap.Core.Api;
using LightNap.Core.Extensions;
using LightNap.Core.Interfaces;
using LightNap.DataProviders.Sqlite.Extensions;
using LightNap.DataProviders.SqlServer.Extensions;
using LightNap.MaintenanceService;
using LightNap.MaintenanceService.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddLogging(configure => configure.AddConsole());

        string databaseProvider = context.Configuration.GetRequiredSetting("DatabaseProvider");
        switch (databaseProvider)
        {
            case "InMemory":
                services.AddLightNapInMemoryDatabase();
                break;
            case "Sqlite":
                services.AddLightNapSqlite(context.Configuration);
                break;
            case "SqlServer":
                services.AddLightNapSqlServer(context.Configuration);
                break;
            default: throw new ArgumentException($"Unsupported 'DatabaseProvider' setting: '{databaseProvider}'");
        }

        services.AddScoped<IUserContext, SystemUserContext>();

        // Manage the tasks to run here. All transient dependencies added for IMaintenanceTask will be in the collection passed to MainService.
        services.AddTransient<IMaintenanceTask, CountUsersMaintenanceTask>();
        services.AddTransient<IMaintenanceTask, PurgeExpiredRefreshTokensMaintenanceTask>();

        services.AddTransient<MainService>();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var mainService = services.GetRequiredService<MainService>();
    await mainService.RunAsync();
}