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

## Database

See the options for configuring the [database provider](./database-providers) to use.

## Email

See the options for configuring the [email provider](./email-providers) to use.

## JSON Web Tokens (JWT)

See the options for configuring the [JSON Web Tokens](./configuring-jwt) used for site authentication.

## Seeding Administrators and other users

See the options for [seeding user accounts](./seeding-users).
