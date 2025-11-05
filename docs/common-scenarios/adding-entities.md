---
title: Adding Entities
layout: home
parent: Common Scenarios
nav_order: 100
---

# {{ page.title }}

LightNap makes use of [Entity Framework (EF)](https://learn.microsoft.com/en-us/ef/core/modeling/) for database access. The `ApplicationDbContext` and supporting entities are all kept in the `Data` folder of the `LightNap.Core` project.

## Adding a New Entity

The process for changing the EF model by adding, updating, or removing is straightforward. Just add/edit/remove classes and references and generate a migration.

1. **Add the Entity Class**: Create a new class in the `Entities` folder that represents the new entity.

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

2. **Update DbContext**: Add `DbSet<TEntity>` properties to the `ApplicationDbContext` class for each new entity.

    ```csharp
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
      public DbSet<TestEntity> TestEntities { get; set; } = null!;
    ```

3. **Configure Relationships**: Define relationships and constraints using Fluent API or data annotations in your entity classes.
4. **Migrations**: Use [EF migrations](../getting-started/database-providers/ef-migrations) to update the database schema.

    {: .note }
    It's recommended to use the [in-memory](../getting-started/database-providers/in-memory-provider) database provider while working out the exact details of the entity. Once the model is finalized a single migration can be generated to keep the process cleaner.

Once the new entity is wired up, check out [our scaffolding support](./scaffolding) to quickly generate code to expose that entity all the way through to the Angular app.

## See Also

- [Adding In-App Notification Types](./adding-notifications) - Example of working with the Notification entity
- [Scaffolding From an Entity](./scaffolding) - Generate complete infrastructure from an entity
