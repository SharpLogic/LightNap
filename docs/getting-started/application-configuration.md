---
title: Application Configuration
layout: home
parent: Getting Started
nav_order: 200
---

# {{ page.title }}

All backend configuration can be done from the `appsettings.json` in `LightNap.WebApi`.

{: .important}

In a production deployment it is preferable to define these settings in a secure place, like [Azure app service environment variables](https://learn.microsoft.com/en-us/azure/app-service/reference-app-settings).

## Application Settings

See the options for configuring [application settings](./configuring-application-settings).

## Cache

LightNap uses hybrid caching for performance optimization. Configure cache settings in the `Cache` section of `appsettings.json`.

| Setting | Purpose |
|---------|---------|
| `ExpirationMinutes` | The default expiration time for cached items in minutes. |

## Distributed Backend Support

LightNap supports running multiple backend instances for scalability and high availability. When enabled, Redis is used for distributed caching and SignalR backplane.

To enable distributed mode:

1. Set `UseDistributedMode` to `true` in the application settings.
2. Configure the Redis connection string in the `ConnectionStrings` section:

   ```json
   "ConnectionStrings": {
     "Redis": "your-redis-connection-string"
   }
   ```

In distributed mode, database migrations and seeding are coordinated across instances using Redis-based locking to prevent conflicts.

## Database

See the options for configuring the [database provider](./database-providers) to use.

## Email

See the options for configuring the [email provider](./email-providers) to use.

## JSON Web Tokens (JWT)

See the options for configuring the [JSON Web Tokens](./configuring-jwt) used for site authentication.

## Seeding Administrators and other users

See the options for [seeding user accounts](./seeding-users).
