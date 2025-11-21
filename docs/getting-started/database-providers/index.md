---
title: Database Providers
parent: Getting Started
layout: home
nav_order: 400
---

# {{ page.title }}

LightNap uses Entity Framework Core for data access and supports multiple database providers. Choose the provider that best fits your deployment scenario and development needs.

## Available Providers

### [SQL Server](./sql-server-provider)

Microsoft's enterprise-grade relational database system. Recommended for production deployments in Azure or on-premises environments.

**Best For**: Production applications, enterprise environments, Azure deployments

**Configuration**: Requires connection string to SQL Server instance

### [SQLite](./sqlite-provider)

A lightweight, file-based relational database that doesn't require a separate server process.

**Best For**: Development, testing, small deployments, demos

**Configuration**: Requires file path for database file

### [In-Memory](./in-memory-provider)

A non-persistent database that exists only in memory. Data is lost when the application restarts.

**Best For**: Development, unit testing, rapid prototyping

**Configuration**: No connection string needed; data resets on restart

## Working with Migrations

### [Entity Framework Migrations](./ef-migrations)

Learn how to create, apply, and manage Entity Framework migrations to keep your database schema in sync with your entity models.

**Important**: Migrations must be created separately for each provider (SQL Server and SQLite) when making schema changes.

## Choosing a Provider

| Scenario | Recommended Provider |
|----------|---------------------|
| Production deployment | SQL Server |
| Local development | SQLite or In-Memory |
| Unit/Integration tests | In-Memory |
| Rapid prototyping | In-Memory |
| Simple deployments | SQLite |
| Azure hosting | SQL Server |

## Configuration

Database providers are configured in `appsettings.json` using the `Database:Provider` setting:

```json
{
  "Database": {
    "Provider": "SqlServer"  // or "Sqlite" or "InMemory"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=..."
  }
}
```

See [Application Configuration](../application-configuration/index) for more details.

## Switching Providers

To switch between providers:

1. Update the `Database:Provider` setting in `appsettings.json`
2. Update the connection string (if applicable)
3. Apply migrations for the new provider (if using SQL Server or SQLite)
4. Restart the application

{: .note }
When switching between providers, you'll need to re-seed any required data as the databases are independent.
