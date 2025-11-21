---
title: Maintenance Service
layout: home
parent: Console Apps
nav_order: 251
---

# {{ page.title }}

The MaintenanceService is a .NET console application that provides core functionality for running regular maintenance tasks on the LightNap backend. It is designed to be run as a webjob (e.g., Azure WebJob) or scheduled task (e.g., cron job) and serves as a prescriptive example of how to build console applications for the platform.

## Overview

The MaintenanceService demonstrates best practices for creating lightweight console apps that interact with the LightNap backend. It performs periodic cleanup and reporting tasks, such as purging expired refresh tokens and counting registered users. By keeping the console app thin and delegating core logic to services in the `LightNap.Core` project, it ensures functionality is properly tested and maintained—similar to how the web API provides a thin layer over service access.

This approach allows maintenance tasks to be developed, tested, and versioned alongside the main application code, while the console app itself remains a simple orchestrator.

## Architecture

The service uses a task-based architecture with dependency injection:

- **IMaintenanceTask Interface**: Defines tasks with a `Name` property and `RunAsync()` method.
- **MainService**: Orchestrates task execution sequentially, with logging and error handling.
- **Tasks**: Implement `IMaintenanceTask` and perform specific operations using injected services.

Tasks are registered in `Program.cs` and run in order during startup.

## Setup and Dependencies

Prerequisites:

- .NET 10.0 SDK
- Access to the LightNap database (configured via appsettings.json or environment variables)

Clone the repository and build the project:

```bash
dotnet build src/LightNap.MaintenanceService/LightNap.MaintenanceService.csproj
```

The service depends on `LightNap.Core` for services like `IRefreshTokenService` and database context.

## Configuration

Configure database settings in `appsettings.json` or environment variables, matching the web API configuration. Supported providers: SQL Server, SQLite, In-Memory.

Example `appsettings.json`:

```json
{
  "Database": {
    "Provider": "SqlServer",
    "ConnectionString": "Server=...;Database=...;..."
  }
}
```

See [Configuring Application Settings](../../getting-started/application-configuration/configuring-application-settings.md) for details.

## Implementing Tasks

To add a new task, implement `IMaintenanceTask`:

```csharp
internal class SampleTask(ILogger<SampleTask> logger, ISomeService service) : IMaintenanceTask
{
    public string Name => "Sample Task";

    public async Task RunAsync()
    {
        logger.LogInformation("Running sample task...");
        await service.DoSomethingAsync();
    }
}
```

Register it in `Program.cs`:

```csharp
services.AddTransient<IMaintenanceTask, SampleTask>();
```

Keep task logic minimal; delegate to core services for complex operations.

## Running the Service

### Locally

```bash
dotnet run --project src/LightNap.MaintenanceService
```

### As a WebJob

The provided GitHub workflows already package and deploy MaintenanceService as a webjob when publishing to Azure. This is done by building the project into a conventional location understood to be used for webjobs when deploying to Azure as shown in the snippet of `LightNap.MaintenanceService.csproj` below.

```xml
<PropertyGroup>
  ...
  <WebJobName>MaintenanceService</WebJobName>
  <WebJobType>triggered</WebJobType>
  <PublishDir>$(SolutionDir)/publish/App_Data/jobs/$(WebJobType)/$(WebJobName)</PublishDir>
</PropertyGroup>
```

| Property | Description |
|----------|-------------|
| `WebJobName` | The name of the webjob in Azure. Ensure unique names across webjobs in your project. |
| `WebJobType` | Set to `triggered` for scheduled runs or `continuous` for always-on jobs. |
| `PublishDir` | The output directory for publishing, following Azure's webjob folder structure. |

`WebJobType` is set to `triggered` by default and uses `settings.job` to run on a schedule of every 15 minutes. Update this file and redeploy to change the schedule. The other option is `continuous` and will start the job immediately and restart it if it exits.

If you change the name of the build target or copy this project as a starting point for another webjob, be sure to update `run.sh` to reflect the new name as it's used by Azure to start service runs when on Linux.

### Via Docker

Build and run the included Dockerfile:

```bash
docker build -t maintenanceservice src/LightNap.MaintenanceService
docker run --env-file .env maintenanceservice
```

## Best Practices and Patterns

- **Thin Layer**: Keep console apps lightweight; implement functionality in `LightNap.Core` services for testing and reuse.
- **Dependency Injection**: Use DI for services, logging, and configuration.
- **Error Handling**: Wrap task execution in try-catch with logging.
- **Configuration**: Follow the same patterns as the web API.
- **Testing**: Unit test core services; integration test tasks if needed.

This pattern applies to other console apps, such as data migration tools or batch processors.

## Troubleshooting

- **Database Connection Errors**: Verify connection string and provider settings.
- **Task Failures**: Check logs for exceptions; ensure core services are accessible.
- **Scheduling Issues**: Confirm webjob configuration or cron setup.

For more on deployment, see [Deploy to Azure](../../deployment-and-cicd/cicd-workflows/deploy-to-azure.md).
