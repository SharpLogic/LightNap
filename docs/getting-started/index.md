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

### [Application Configuration](./application-configuration/index)

Overview of all configuration options available in `appsettings.json`, including links to detailed configuration guides for each subsystem.

### [Configuring Application Settings](./application-configuration/configuring-application-settings)

Configure core application behavior including automatic migrations, email verification requirements, two-factor authentication, and cookie settings.

### [Configuring JSON Web Tokens (JWT)](./application-configuration/configuring-jwt)

Set up JWT authentication parameters including the secret key, issuer, audience, and token expiration times. **Critical for production security.**

### [Configuring Rate Limiting](./configuring-rate-limiting)

Configure rate limiting policies to protect your API from abuse, including fixed window limiters, partitioned rate limiting, and custom policies. **Essential for production security and performance.**

### [Seeding Users](./application-configuration/seeding-users)

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

## Authentication Providers

### [OAuth Providers](./oauth-providers)

Enable third-party authentication using OAuth. Allow users to sign in with Google, Microsoft, GitHub, and other providers.

- **[Google OAuth](./oauth-providers#setting-up-google-oauth)** - Sign in with Google accounts
- **[Microsoft OAuth](./oauth-providers#setting-up-microsoft-oauth)** - Sign in with Microsoft/Azure AD accounts
- **[GitHub OAuth](./oauth-providers#setting-up-github-oauth)** - Sign in with GitHub accounts

### [API / Swagger](./swagger)

Explore the API surface via Swagger UI and learn how to authorize with a JWT so you can test protected endpoints interactively.

## Next Steps

Once you have LightNap running:

1. Review the [Concepts](../concepts) section to understand the architecture
2. Explore [Development Guide](../development-guide) for practical implementation guides
3. Check out the [Deployment & CI/CD](../deployment-and-cicd) for CI/CD setup
