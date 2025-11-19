---
title: Solution & Project Structure
layout: home
parent: Concepts
nav_order: 100
---

# {{ page.title }}

LightNap is structured as a .NET backend and an Angular frontend. While they are designed to work together, they are abstracted by a REST API layer. As a result, they should be thought of as two distinct solutions that are developed, built, and run separately.

## Backend Projects

The backend is developed on .NET using C#. `LightNap.sln` includes the projects required to build and run from an IDE like Visual Studio 2022.

- `LightNap.Core`: .NET shared library for common server-side components.
- `LightNap.Core.Tests`: Test library for `LightNap.Core`.
- `LightNap.DataProviders.Sqlite`: SQLite data provider implementation including migrations and utilities.
- `LightNap.DataProviders.SqlServer`: SQL Server data provider implementation including migrations and utilities.
- `LightNap.MaintenanceService`: .NET console project to run maintenance tasks.
- `LightNap.WebApi`: .NET Web API project.

LightNap supports distributed deployment with multiple backend instances. When `UseDistributedMode` is enabled in the configuration, Redis is used for distributed caching and SignalR backplane to coordinate between instances.

### Endpoints

The `LightNap.WebApi` project is a thin layer that exposes REST API endpoints and provides a layer of configuration and authorization. Each controller endpoint hands off the request to a core service method after performing initial validation, so there should be no real business logic contained in this project.

There are four Web API controllers by default. They are organized from the perspective of the API consumer:

1. **IdentityController** (`/api/identity`): Covers authentication, such as login, registration, password reset, and so on. Under typical scenarios these features will not require special attention from developers unless they are extending support for new scenarios, such as two-factor via SMS, OAuth login, etc.

2. **UsersController** (`/api/users`): Covers user administration features, such as searching users, managing roles & claims, deleting users, and so on.

3. **MeController** (`/api/users/me`): Covers logged-in user features, such as setting profile data, managing devices & settings, and [in-app notifications](../development-guide/features/adding-notifications).

4. **PublicController** (`/api/public`): Covers business features intended for any user, including those who are not logged in. This infrastructure is provided as a series of stubs to be extended for the application scenario.

It is expected that developers add additional controllers to organize endpoints, such as something like `ProductsController` for `/api/products/`, as needed for business domain functionality.

#### Minimal API Philosophy

To minimize the number of endpoints exposed, functionality that may vary in behavior across users based on data privileges share the same endpoint. For example, `/api/users/search` is the only endpoint for searching users.

- When invoked by an `Administrator`, all valid parameters are included and all relevant user data is returned.
- When invoked by an authenticated user, only a subset of all possible parameters are included and limited user data is returned.
- When invoked by an anonymous users, a very limited set of parameters are included and the minimum necessary user data is returned.

The pattern for this implementation can be reviewed and adjusted in the source. It's recommended to avoid providing separate endpoints on a per-privilege basis as it produces additional surface area for the API and services without significantly reducing the complexity.

### Core Services

All business logic should occur within the `LightNap.Core` services. These are organized by domain functionality. While they may often align with controller functionality, they're primarily organized based on the resources they manage and refactored to minimize complexity.

While the Web API authorization is very effective, it's strongly encouraged that developers practice defense in depth by validating security requirements within the `LightNap.Core` services that fulfill application functionality. See the `IUserContext` interface for methods to validate the authentication, role, and claim requirements of the current user behind every request.

## Frontend Project

The frontend is developed on Angular using Typescript.

- `lightnap-ng`: Angular project with PrimeNG components based on an early version of the [sakai-ng](https://github.com/primefaces/sakai-ng) template. It has since been modified significantly and doesn't really match up to any version of the sakai-ng project anymore. However, that project (maintained by PrimeTek) is a great place to explore the latest version of PrimeNG features in action.

### Folder Structure

Most of the stuff outside `/src/app` is standard platform functionality, so we'll just focus on the content inside.

#### The `core` Folder

The `core` folder contains everything functional built in to the base LightNap user experience. Most of these folders contain what you'd expect to see from an Angular project, but there are a few special cases that require a little more explanation.

The `backend-api` folder that contains REST API integration services and DTOs.

- `dtos`: DTOs that map pretty closely to those in the `LightNap.Core` backend DTOs, so if you change something on the backend you'll want to update it here as well.
- `helpers`: Helper classes for rehydrating data pulled down from the backend, which is usually just `Date` objects.
- `services`: Thin layers to wrap backend HTTP requests. If you add or update endpoints on existing backend controllers, update them here as well. It's recommended that these data services never be used directly by application code, but are rather accessed through an application service described below.

There are also a variety of application services that are exposed in the `services` folder or from the `services` folders under functional areas, such as the `notifications` or `users` folder. Some of these wrap the data services in the `backend-api` folder and are the recommended way to access those features from application code. This level of abstraction provides a better way to handle pre- and post-processing of requests and responses, as well as to compose or otherwise translate raw DTOs into more sophisticated entities. Learn more about [adding notification types](../development-guide/features/adding-notifications).

#### The `pages` Folder

The `pages` folder contains a hierarchy of area pages for the application. While they don't need to map exactly to application routes, they generally do. For example, `src/pages/admin/index` contains the view rendered at the route `/admin`. Each area folder is organized with its own `routes.ts` defining route details for its pages. Check out the [route alias concept](../development-guide/frontend-customization/using-route-alias) used by LightNap.

#### The `testing` Folder

The `testing` folder contains utilities and helpers for testing the Angular application.

- `builders`: Factory functions for creating test data objects.
- `mocks`: Mock services and data for isolating components during testing.
- `matchers`: Custom Jasmine matchers for assertions.
- `test-helpers.ts`: Common testing utilities and setup functions.
- `test.module.ts`: A testing module with common providers.

This structure supports comprehensive unit and integration testing throughout the application.

## Scaffolder

There is also a `Scaffolding` folder that contains the source to [the scaffolder tool](../development-guide/data-persistence/scaffolding). It is not necessary to build or run this as part of the LightNap solution.

## Data Flow

This solution is architected in a consistent way across all major areas. For any given feature it's implemented such that the end user interacts with Angular components that call into Angular services. Those services send HTTP requests to the REST endpoints hosted in Web API controllers. Those controllers relay requests to services that access the database via Entity Framework.

 ```mermaid
  graph TD
    subgraph Database
      DB[(Database)]
    end

    subgraph Backend
      Core[LightNap.Core services]
      WebAPI[LightNap.WebApi controllers]
    end

    subgraph Frontend
      AngularService[lightnap-ng services]
      UIComponents[lightnap-ng components]
    end

    Core -.-> |Entity Framework| DB
    WebAPI --> Core
    AngularService -.-> |REST| WebAPI
    UIComponents --> AngularService
```

### Profile Example

Consider the scenario where a user navigates to their profile page.

 ```mermaid
  graph TD
    subgraph Database
      DB[(Database)]
    end

    subgraph Backend
      Core["ProfileService"]
      WebAPI["MeController"]
    end

    subgraph Frontend
      ProfileDataService[ProfileDataService]
      ProfileService[ProfileService]
      ProfileComponent["(Profile) IndexComponent"]
    end

    Core -.-> |Entity Framework| DB
    WebAPI --> |"ProfileService.GetProfileAsync()"| Core
    ProfileDataService -.-> |GET /api/users/me/profile| WebAPI
    ProfileService --> |"ProfileDataService.getProfile()"| ProfileDataService
    ProfileComponent --> |"ProfileService.getProfile()"| ProfileService
```

This pattern is consistently applied across other areas such that understanding one complete area provides a head start on the others.
