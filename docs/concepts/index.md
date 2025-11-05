---
title: Concepts
layout: home
nav_order: 300
---

# {{ page.title }}

This section explains the core concepts and architectural patterns used throughout LightNap. Understanding these concepts will help you work more effectively with the framework and make informed decisions when extending it.

## Architecture & Design

### [Solution & Project Structure](./project-structure)

Understand how LightNap is organized, including the .NET backend projects, Angular frontend structure, and the data flow pattern that connects them. This is the foundation for understanding how all the pieces fit together.

### [API Response Model](./api-response-model)

Learn about LightNap's standardized REST API response format, including how errors are handled, HTTP status codes are used, and how the frontend automatically processes API responses.

## Authentication & Security

### [Authentication & Tokens](./authentication)

Deep dive into LightNap's JWT-based authentication system, including how access tokens and refresh tokens work, token lifecycle management, and integration with the Angular frontend.

### [Devices](./devices)

Understand the device/session management system that allows users to track and manage their logged-in sessions across different browsers and devices.

## Getting Started

New to LightNap? Start with:

1. **[Solution & Project Structure](./project-structure)** - Get oriented with the codebase
2. **[Authentication & Tokens](./authentication)** - Understand the security model
3. **[API Response Model](./api-response-model)** - Learn the API patterns

Then explore the [Common Scenarios](../common-scenarios) section for practical implementation guides.
