---
title: Getting Started
layout: home
nav_order: 200
---

# {{ page.title }}

This section covers everything you need to get LightNap up and running, from initial setup to production deployment. Follow these guides in order for the smoothest experience, or jump to specific topics as needed.

## Quick Start

### [Building and Running the Application](./building-and-running)

Get LightNap running locally in minutes. This guide covers prerequisites, installation steps, and how to access the application once it's running.

## Configuration

### [Application Configuration](./application-configuration)

Overview of all configuration options available in `appsettings.json`, including links to detailed configuration guides for each subsystem.

### [Configuring Application Settings](./configuring-application-settings)

Configure core application behavior including automatic migrations, email verification requirements, two-factor authentication, and cookie settings.

### [Configuring JSON Web Tokens (JWT)](./configuring-jwt)

Set up JWT authentication parameters including the secret key, issuer, audience, and token expiration times. **Critical for production security.**

### [Seeding Users](./seeding-users)

Bootstrap your application with initial user accounts and assign them to roles. Useful for creating administrator accounts and test users.

## Data Providers

### [Database Providers](./database-providers)

Choose and configure your database provider. LightNap supports SQL Server, SQLite, and in-memory databases for different scenarios.

- **[SQL Server](./database-providers/sql-server-provider)** - Production-ready relational database
- **[SQLite](./database-providers/sqlite-provider)** - Lightweight file-based database
- **[In-Memory](./database-providers/in-memory-provider)** - Fast testing without persistence
- **[Entity Framework Migrations](./database-providers/ef-migrations)** - Managing schema changes

## Email Integration

### [Email Providers](./email-providers)

Configure how LightNap sends transactional emails for password resets, email verification, and two-factor authentication.

- **[SMTP Provider](./email-providers/smtp-provider)** - Production email via SMTP servers
- **[Log To Console Provider](./email-providers/log-to-console-provider)** - Development email logging

## Next Steps

Once you have LightNap running:

1. Review the [Concepts](../concepts) section to understand the architecture
2. Explore [Development Guide](../development-guide) for practical implementation guides
3. Check out the [Deployment & CI/CD](../deployment-and-cicd) for CI/CD setup
