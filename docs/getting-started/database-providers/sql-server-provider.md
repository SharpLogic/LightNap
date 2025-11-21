---
title: SQL Server Provider
parent: Database Providers
layout: home
nav_order: 10
---

# {{ page.title }}

LightNap supports Microsoft SQL Server as a production-ready database provider using Entity Framework Core.

## Prerequisites

- SQL Server 2019 or later (or Azure SQL Database)
- Appropriate connection permissions

## Configuration

Update `appsettings.json`:

```json
{
  "Database": {
    "Provider": "SqlServer"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=LightNap;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Azure SQL Database

For Azure SQL Database:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:your-server.database.windows.net,1433;Initial Catalog=LightNap;Persist Security Info=False;User ID=your-user;Password=your-password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

## Initial Setup

1. Create the database in SQL Server
2. Update connection string in `appsettings.json`
3. Run the application - migrations will apply automatically

## Migrations

SQL Server migrations are stored in `src/LightNap.DataProviders.SqlServer/Migrations/`.

To create a new migration:

```bash
cd src
dotnet ef migrations add MigrationName --project LightNap.DataProviders.SqlServer --startup-project LightNap.WebApi
```

## Best Practices

- Use Azure SQL Database for cloud deployments
- Enable connection pooling for better performance
- Configure appropriate firewall rules
- Use managed identity authentication in Azure
- Monitor query performance with SQL Server Profiler
