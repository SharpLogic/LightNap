---
title: API / Swagger
layout: home
parent: Getting Started
nav_order: 650
---

# {{ page.title }}

LightNap exposes an OpenAPI (Swagger) definition and UI to help developers explore and test the HTTP APIs.

## Accessing Swagger

1. Start the backend: `dotnet run --project LightNap.WebApi`
2. Visit the Swagger UI at `https://localhost:<port>/swagger` (the port the API logs when it starts).

> Swagger is enabled only when the application is running in the Development environment (see `Program.cs`).

## Authorizing with JWT

Many API endpoints require authorization. LightNap adds a "Bearer" security scheme to Swagger so you can run authorized requests directly from the UI.

- Click the "Authorize" button in Swagger UI
- Paste `Bearer <access-token>` (where `<access-token>` is a JWT returned by the Identity endpoints such as `POST /api/identity/login`)

Once authorized you can call protected endpoints directly from Swagger.

## Notes

- If the token expires, re-authorize to refresh testing access through Swagger.
- For production deployments you may want to remove/lock down Swagger.

For more details on obtaining tokens see [Configuring JSON Web Tokens (JWT)](./application-configuration/configuring-jwt)
