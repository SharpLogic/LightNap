---
title: Entity Framework Migrations
layout: home
parent: Database Providers
nav_order: 40
---

# {{ page.title }}

Use the Entity Framework tools to create and apply migrations.

{: .note}

This page will use the `LightNap.DataProviders.SqlServer` migrations project, but the same commands should work by swapping that out for another provider, such as `LightNap.DataProviders.Sqlite`. There is no need to update migrations for database providers that will not be used. There is also no need to use migrations for the in-memory database provider.

The LightNap solution structure spreads out EF assets across multiple projects:

- Entities are defined in the `LightNap.Core` project.
- Migrations are defined in provider-specific projects, such as `LightNap.DataProviders.SqlServer`.
- The Web API entry point and EF configuration are defined in the `LightNap.WebApi` project.

As a result, additional parameters are required for the EF tools to work properly.

Before modifying the migrations for a given project, set the `Database:Provider` key in the `appsettings.json` file of the `LightNap.WebApi` project to the appropriate value (like `SqlServer`).

## Adding a Migration

To add a migration for SQL Server, use the following command from the `/src` folder:

```bash
dotnet ef migrations add <MigrationName> --context ApplicationDbContext --startup-project LightNap.WebApi --project LightNap.DataProviders.SqlServer
```

## Removing a Migration

Similarly, you can remove the most recent migration using the following command:

```bash
dotnet ef migrations remove --startup-project LightNap.WebApi --project LightNap.DataProviders.SqlServer
```

## Updating the Database

To apply changes to the database, use the following command:

```bash
dotnet ef database update --startup-project LightNap.WebApi --project LightNap.DataProviders.SqlServer
```

{: .note}

The `LightNap.WebApi` project also offers automatic migrations by setting
[`Database:AutomaticallyApplyEfMigrations`](../application-configuration/configuring-application-settings) to `true`.
