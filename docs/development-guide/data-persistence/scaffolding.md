---
title: Scaffolding From an Entity
layout: home
parent: Data & Persistence
nav_order: 40
---

# {{ page.title }}

{: .important }
Scaffolding support is experimental. Please be sure to stash or commit changes before running the scaffolder in case it doesn't quite work the way you expect it to.

The scaffolder takes [an entity](./adding-entities) as an input and generates a ton of boilerplate code to make it easier to build and deploy features. This includes all the objects, services, and pages that get data from the database to the browser and back following the patterns used throughout the solution.

- TOC
{:toc}

## Setting Up Your Entity

The scaffolder assumes you're scaffolding [an entity model](./adding-entities) intended for typical CRUD operations in a database. It's not required that the target object be an EF entity, but there is some code in the generated service that expects it.

It's recommended that entities are stored in the `Data\Entities` folder of the `LightNap.Core` project. We'll use `TestEntity` as the example for this tutorial.

```csharp
namespace LightNap.Core.Data.Entities
{
    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
```

Next, we'll add a reference to it in `ApplicationDbContext`:

```csharp
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public DbSet<TestEntity> TestEntities { get; set; } = null!;
    ...
```

Note that it's expected that the `DbSet` will use the pluralized version of the entity name. If you don't have it set up this way then some of the generated code won't compile without minor edits.

## Running the Scaffolder

At this time, the scaffolder is provided in source form in the `src/Scaffolding` folder. You'll need to build it before you can run it.

```bash
  cd src
  dotnet build ./Scaffolding
  .\Scaffolding\LightNap.Scaffolding\bin\Debug\net10.0\LightNap.Scaffolding.exe TestEntity
```

### Scaffolder options

The scaffolder accepts additional CLI options to customize output. The commonly-used options are:

- `--namespace` — The C# namespace for the generated entities (default: `LightNap.Core.Data.Entities`).
- `--src-path` — Path to the repository `src` directory (default: `./`).
- `--core-project` — Core project name (default: `LightNap.Core`).
- `--web-api-project` — Web API project name (default: `LightNap.WebApi`).
- `--angular-project` — Angular app name (default: `lightnap-ng`).
- `--skip-components` — Skip generating Angular components (default: `false`).
- `--overwrite` — Overwrite existing files automatically (default: `false`).

These match the options exposed by the newest scaffolder experience added in this branch.

{: .note }
The script above is run from the repo's main `src` directory to cleanly align with the default parameters. You can override most of these parameters if needed.

## Generated Files

Most new files are generated to create a new area based around the provided entity. Instead of changing content within the other areas (`Profile`, `Admin`, etc.) the new area will match the entity name (like `TestEntity`) and provide a starting point for integration. It's up to the developer to decide whether and which items are merged into other areas or kept contained within the newly created area.

In the Core project, everything is added in a new `TestEntities` area folder:

| File                                 | Purpose                                                   |
| :----------------------------------- | :-------------------------------------------------------- |
| `Dto/Response/TestEntityDto.cs`      | Returned from get or search requests.                     |
| `Dto/Request/CreateTestEntityDto.cs` | Parameter for create requests.                            |
| `Dto/Request/SearchTestEntityDto.cs` | Parameter for search requests.                            |
| `Dto/Request/UpdateTestEntityDto.cs` | Parameter for update requests.                            |
| `Interfaces/ITestEntityService.cs`   | Interface for the area service.                           |
| `Services/TestEntityService.cs`      | Implementation of the area service.                       |

The only core service file not stored in the new area folder the extension methods class for mapping between the entity and DTOs, `TestEntityExtensions.cs`. It's created alongside all other extension classes in the root `Extensions` folder.

In the Web API project:

| File                                    | Purpose                                                 |
| :-------------------------------------- | :------------------------------------------------------ |
| `Controllers/TestEntitiesController.cs` | The Web API endpoints that map to core service methods. |

In the Angular project, everything is added in a new `app/test-entities` area folder:

| File                                             | Purpose                                             |
| :----------------------------------------------- | :-------------------------------------------------- |
| `components/pages/create/create.component.html`  | Markup for the create item page.                    |
| `components/pages/create/create.component.ts`    | Code for the create item page.                      |
| `components/pages/get/get.component.html`        | Markup for the get item page.                       |
| `components/pages/get/get.component.ts`          | Code for the get item page.                         |
| `components/pages/edit/edit.component.html`      | Markup for the edit item page.                      |
| `components/pages/edit/edit.component.ts`        | Code for the edit item page.                        |
| `components/pages/index/index.component.html`    | Markup for the area landing/search page.            |
| `components/pages/index/index.component.ts`      | Code for the area landing/search page.              |
| `components/pages/routes.ts`                     | Relative routes for the area pages.                 |
| `helpers/test-entity.helper.ts`                  | Rehydrates dates from DTOs if needed.               |
| `models/response/test-entity.ts`                 | Maps to the server's `TestEntityDto`.               |
| `models/request/create-test-entity-request.ts`   | Maps to the server's `CreateTestEntityDto`.         |
| `models/request/search-test-entities-request.ts` | Maps to the server's `SearchTestEntityDto`.         |
| `models/request/update-test-entity-request.ts`   | Maps to the server's `UpdateTestEntityDto`.         |
| `services/data.service.ts`                       | The private service for HTTP calls to the REST API. |
| `services/test-entity.service.ts`                | The area service for app usage.                     |

## Final Configuration

After the scaffolder completes there are still a couple of things that need to be done for it to be ready for testing.

### Registering the Core Service For the Web API Controller

The new Web API controller expects to have the new Core service injected. This needs to be configured in the `AddApplicationServices` method of `Extensions/ApplicationServiceExtensions.cs` and will look something like this:

```csharp
public static IServiceCollection AddApplicationServices(this IServiceCollection services)
{
  ...
  services.AddScoped<ITestEntityService, TestEntityService>();
  ...
```

Now you can [build and run](../../getting-started/building-and-running) the backend.

### Registering the Area Routes in the Angular Frontend

The new Angular area routes need to be added to the root route tree in `app/pages/routes.ts` so that the area pages can be accessed.

First, add the import of the routes at the top:

```typescript
import { Routes as TestEntityRoutes } from "../test-entities/components/pages/routes";
```

Next, add the routes themselves to the tree. For example, if you want them to only be accessible to logged-in users, put them after the `profile` routes:

```typescript
...
children: [
  { path: "home", data: { breadcrumb: "Home" }, children: UserRoutes },
  { path: "profile", data: { breadcrumb: "Profile" }, children: ProfileRoutes },
  { path: "test-entities", children: TestEntityRoutes }
  ...
```

Now you can [build and run](../../getting-started/building-and-running) the frontend. You can access the new area at a URL like `http://localhost:4200/test-entities`.

## Undoing the Scaffolder

The scaffolder only creates files, so deleting those files will undo its work.

## Updating Scaffolding

The scaffolder does not support updating existing files. It will exit without making changes if there are any files already in locations it plans to write to.

## See Also

- [Adding Entities](./adding-entities) - Manual process for adding entities
- [Adding In-App Notification Types](../features/adding-notifications) - Example of implementing a feature without scaffolding
- [Solution & Project Structure](../../concepts/project-structure) - Understanding the architecture
