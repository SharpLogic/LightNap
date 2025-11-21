---
title: SQLite Provider
parent: Database Providers
layout: home
nav_order: 20
---

# {{ page.title }}

LightNap supports SQLite as a lightweight, file-based database provider using Entity Framework Core.

## Prerequisites

- No additional software required (SQLite is embedded)

## Configuration

Update `appsettings.json`:

```json
{
  "Database": {
    "Provider": "Sqlite"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=LightNap.db"
  }
}
```

## Initial Setup

1. Update connection string in `appsettings.json`
2. Run the application - database file and migrations will be created automatically

## Migrations

SQLite migrations are stored in `src/LightNap.DataProviders.Sqlite/Migrations/`.

To create a new migration:

```bash
cd src
dotnet ef migrations add MigrationName --project LightNap.DataProviders.Sqlite --startup-project LightNap.WebApi
```

## Best Practices

- Use for development and small deployments
- Database file is portable and can be committed to version control
- No server process required
- Good for testing and demos
- Consider file locking limitations for concurrent access
