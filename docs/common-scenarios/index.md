---
title: Common Scenarios
layout: home
nav_order: 400
---

# {{ page.title }}

This section provides practical, step-by-step guides for common development tasks in LightNap. Each article walks you through implementing a specific feature or customization, following the established patterns used throughout the application.

## Data & Persistence

### [Adding Entities](./adding-entities)

Learn how to add new database entities using Entity Framework, including creating entity classes, updating the DbContext, and generating migrations.

### [Adding Profile Fields](./adding-profile-fields)

Extend the user profile with custom fields. This comprehensive example covers the full stack from backend entities to frontend forms, demonstrating the complete data flow pattern.

### [Scaffolding From an Entity](./scaffolding)

Use the scaffolding tool to automatically generate boilerplate code for CRUD operations, including backend services, API controllers, and Angular components.

## Security & Authorization

### [Working With Roles](./working-with-roles)

Add and manage application roles for authorization. Learn how to define roles, apply them to endpoints, and use them in the frontend to control access.

### [Working With Custom Claims](./custom-claims)

Implement fine-grained permissions using ASP.NET claims. This guide covers claim-based authorization for dynamic, record-level security.

## Features

### [Adding In-App Notification Types](./adding-notifications)

Create new types of in-app notifications to keep users informed about events and activities. Learn how to trigger notifications from backend services and display them in the Angular UI.

### [Adding User Settings](./adding-user-settings)

Implement user-specific settings and preferences that persist across sessions. This guide covers the full stack from backend storage to frontend controls, demonstrating how to add boolean, numeric, and string settings using the ProfileService and PrimeNG components.

## Frontend Customization

### [Working With Angular Routes](./using-route-alias)

Understand LightNap's route alias system that makes it easier to manage and refactor Angular routes throughout the application.

### [Updating the Sidebar Menu](./sidebar-menu)

Customize the sidebar navigation menu to add new sections, items, and role-based visibility.
