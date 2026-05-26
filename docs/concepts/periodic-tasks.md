---
title: Periodic and Background Tasks
layout: home
parent: Concepts
nav_order: 1200
---

# {{ page.title }}

LightNap distinguishes one-shot batch maintenance (the `LightNap.MaintenanceService` project) from continuous in-process background work. Most consumers don't need a new abstraction for periodic work — they need a scheduled invocation of an existing one-shot task or a long-running `IHostedService` inside the WebApi.

There is no `IPeriodicTask` interface in LightNap, and there does not need to be one. The two existing patterns cover the cases that come up in practice; the section below lays out which to pick.

## One-shot batch maintenance

Use `LightNap.MaintenanceService` when your work is:

- Nightly or weekly (refresh-token purge, audit-log purge).
- Triggered externally (cron, Azure WebJob, GitHub Action, Kubernetes CronJob).
- Acceptable to run in a separate process that exits when done.

To add a new task: implement `IMaintenanceTask` and register it alongside the existing tasks in `LightNap.MaintenanceService/Program.cs`.

```csharp
internal class PurgeExpiredWidgetsMaintenanceTask(
    ILogger<PurgeExpiredWidgetsMaintenanceTask> logger,
    ApplicationDbContext db) : IMaintenanceTask
{
    public string Name => "Purge Expired Widgets";

    public async Task RunAsync()
    {
        var cutoff = DateTime.UtcNow.AddDays(-30);
        var expired = await db.Widgets.Where(w => w.CreatedAt < cutoff).ToListAsync();
        db.Widgets.RemoveRange(expired);
        await db.SaveChangesAsync();
        logger.LogInformation("Removed {Count} expired widgets", expired.Count);
    }
}
```

```csharp
// Program.cs
services.AddTransient<IMaintenanceTask, PurgeExpiredWidgetsMaintenanceTask>();
```

`MainService` runs every registered `IMaintenanceTask`, so registering it is the entire wiring step.

## Continuous in-process background work

For work that needs to run while the WebApi is up (polling a third-party API every 10 minutes, batched notification sends every 5 minutes, queue consumers), register an `IHostedService` directly in your consumer's WebApi project. `System.Threading.PeriodicTimer` covers the cadence cleanly in .NET 8+; it does not pull in Quartz/Hangfire-shaped dependencies for the common case.

```csharp
public sealed class WidgetPollerService(
    IServiceProvider services,
    ILogger<WidgetPollerService> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(10);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);
        do
        {
            try
            {
                using var scope = services.CreateScope();
                var widgetService = scope.ServiceProvider.GetRequiredService<IWidgetService>();
                await widgetService.PollAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Shutdown — bail out cleanly.
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Widget poll iteration failed; retrying on next tick");
            }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}

// Register in Program.cs:
builder.Services.AddHostedService<WidgetPollerService>();
```

A few defaults worth keeping:

- Always catch and log per-iteration exceptions so a transient failure doesn't kill the service.
- Always create a DI scope per iteration when invoking scoped services (your domain services, the DbContext) — `IHostedService` itself is a singleton.
- Honor the cancellation token at every async boundary so shutdown completes promptly.
- For work that should not overlap with itself, lean on `WaitForNextTickAsync` — it skips a tick rather than queueing a second run if the previous one is still running.

## When to consider an external scheduler

Pull in Quartz, Hangfire, or Azure Functions when you need:

- Persistent job storage (jobs survive a process restart with their state intact).
- Distributed coordination across instances (only one instance runs the job at a time).
- Complex schedules (cron expressions, dependency chains, manual retries with backoff state).

LightNap doesn't ship any of these because most consumers don't need them. The `MaintenanceService` + `IHostedService` patterns cover the rest of the cases cleanly.

## Configuration fail-fast

A related concern: `LightNap.WebApi` requires a database provider to be set in configuration. The `DatabaseProvider.Unconfigured` sentinel is the default value used to detect a missing `Database:Provider` setting. `AddDatabaseServices` throws an `ArgumentException` with the message `"Unsupported 'Database:Provider' setting: 'Unconfigured'"` when this happens, so a fresh clone with no Database section in configuration fails immediately at startup rather than silently picking a default. This behavior is verified by `DatabaseProviderUnconfiguredTests`.
