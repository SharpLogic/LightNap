---
title: Backend Seeding
layout: home
parent: Data & Persistence
nav_order: 10
---

# {{ page.title }}

LightNap provides a comprehensive seeding system that runs automatically on application startup. This system ensures that essential data like roles, users, and application content is present in the database before the application becomes available.

## How Seeding Works

The seeding process is triggered during application startup in `Program.cs`, immediately after database migrations are applied:

1. **Database Migration** - Entity Framework migrations are applied (if configured)
2. **Seeding Execution** - The `Seeder` class runs through its seeding pipeline
3. **Application Start** - The application becomes available to handle requests

The seeding process runs **every time the application starts**, making it idempotent—it safely checks for existing data and only creates what's missing.

## The Seeding Pipeline

The `Seeder` class executes the following steps in order:

### 1. Seed Roles (`SeedRolesAsync`)

Creates all application roles defined in `ApplicationRoles.All` and removes any roles not defined in the application. This ensures the role set matches your application's configuration.

### 2. Seed Users (`SeedUsersAsync`)

Creates user accounts based on the `SeededUsers` configuration in `appsettings.json` and assigns them to their designated roles. See [Seeding Users](../getting-started/seeding-users) for detailed configuration options.

### 3. Seed Static Content (`SeedStaticContentAsync`)

Loads static content from the file system (`StaticContent` directory) into the database. This includes:

- **Zones** - Content areas within pages
- **Pages** - Full page content

Content is organized by:

- **Type** - Zone or Page
- **Access Level** - Public, Authenticated, or Explicit
- **Key** - Kebab-case identifier
- **Language** - Two-letter language code (e.g., `en`, `es`)
- **Format** - HTML, Markdown, or Plain Text

### 4. Seed Application Content (`SeedApplicationContentAsync`)

Provides a hook for custom application-wide seeding logic that should run in **all environments**. This method is intentionally left empty by default.

```csharp
private Task SeedApplicationContentAsync()
{
    // TODO: Add any seeding code you want run every time
    // the app loads in any environment.
    return Task.CompletedTask;
}
```

Use this for seeding data that is essential to your application regardless of whether it's running in development, staging, or production.

### 5. Seed Environment Content (`SeedEnvironmentContentAsync`)

Executes environment-specific seeding logic based on the current hosting environment. This method directly implements seeding for different environments:

- **Development** - Test data for local development
- **E2e** - Data required for end-to-end testing
- **Staging** - Staging-specific data that mimics production
- **Production** - Production-specific initialization (use sparingly)

### 6. Seed Local Content (`SeedLocalContentAsync`)

Calls the optional `SeedLocalContent()` partial method, which can be implemented in `Seeder.Local.cs` for developer-specific local seeding that should not be committed to source control.

## Environment-Specific Seeding

LightNap's seeding system supports environment-specific seeding through direct methods in the `Seeder` class. This allows you to include development and testing data without risking it being deployed to production.

### How It Works

The `SeedEnvironmentContentAsync` method checks the current hosting environment and calls the appropriate seeding method:

```csharp
public async Task SeedEnvironmentContentAsync()
{
    var environment = serviceProvider.GetRequiredService<IHostEnvironment>();

    if (environment.EnvironmentName == "Development")
    {
        await this.SeedDevelopmentContentAsync();
    }
    else if (environment.EnvironmentName == "E2e")
    {
        await this.SeedE2eContentAsync();
    }
    else if (environment.EnvironmentName == "Staging")
    {
        await this.SeedStagingContentAsync();
    }
    else if (environment.EnvironmentName == "Production")
    {
        await this.SeedProductionContentAsync();
    }
}
```

### Environment-Specific Methods

Each environment has its own private method in the `Seeder` class:

#### SeedDevelopmentContentAsync

```csharp
private async Task SeedDevelopmentContentAsync()
{
    logger.LogInformation("Seeding Development environment content");

    // Add Development-specific seeding logic here
    // This is for data that should be committed to source control
    // and useful for most/all developers working on this project

    logger.LogInformation("Seeded Development environment content");
}
```

#### SeedE2eContentAsync

```csharp
private async Task SeedE2eContentAsync()
{
    logger.LogInformation("Seeding E2E test content");
    
    // Seed data required for end-to-end tests
    await contentService.CreateStaticContentAsync(
        new CreateStaticContentDto()
        {
            Key = "e2e-test-page",
            Type = StaticContentType.Page,
            Status = StaticContentStatus.Published,
            ReadAccess = StaticContentReadAccess.Public
        }
    );
    
    logger.LogInformation("Seeded E2E test content");
}
```

#### SeedStagingContentAsync

```csharp
private async Task SeedStagingContentAsync()
{
    logger.LogInformation("Seeding Staging environment content");

    // Add Staging-specific seeding logic here

    logger.LogInformation("Seeded Staging environment content");
}
```

#### SeedProductionContentAsync

```csharp
private async Task SeedProductionContentAsync()
{
    logger.LogInformation("Seeding Production environment content");

    // Add Production-specific seeding logic here

    logger.LogInformation("Seeded Production environment content");
}
```

## Local-Only Seeding

For developer-specific seeding that should not be committed to source control, LightNap provides a partial method pattern through `Seeder.Local.cs`.

### How It Works

The `Seeder` class includes an optional partial method:

```csharp
partial void SeedLocalContent();
```

You can implement this method in a separate file that is excluded from source control:

#### Seeder.Local.cs

```csharp
namespace LightNap.WebApi.Configuration
{
    public partial class Seeder
    {
        private ApplicationDbContext _db = serviceProvider.GetRequiredService<ApplicationDbContext>();

        partial void SeedLocalContent()
        {
            this.SeedLocalContentInternalAsync().Wait();
        }

        private async Task SeedLocalContentInternalAsync()
        {
            // Seed local-only data for your specific development scenario
            if (!_db.SomeEntities.Any())
            {
                _db.SomeEntities.Add(new SomeEntity { Name = "Local Test Data" });
                await _db.SaveChangesAsync();
            }
        }
    }
}
```

{: .note }
Add `Seeder.Local.cs` to your `.gitignore` file to prevent it from being committed to source control. This allows each developer to maintain their own local seeding logic without affecting others.

## System User Context

During seeding, the application uses a special `SystemUserContext` instead of the normal `IUserContext`. This is necessary because:

1. **No authenticated user exists** during application startup
2. **Elevated privileges** are required to create roles and users
3. **Authorization checks** would otherwise fail

The `SystemUserContext`:

- Always reports as authenticated and an administrator
- Has all roles and claims
- Returns `"system"` as the user ID
- Bypasses all authorization checks

This context is configured in `Program.cs`:

```csharp
// Replace IUserContext with SystemUserContext for seeding
var seederServiceCollection = new ServiceCollection();
foreach (var descriptor in builder.Services.Where(descriptor => descriptor.ServiceType != typeof(IUserContext)))
{
    seederServiceCollection.Add(descriptor);
}
seederServiceCollection.AddScoped<IUserContext, SystemUserContext>();
seederServiceCollection.AddScoped<Seeder>();

using var seederServiceProvider = seederServiceCollection.BuildServiceProvider();
var seeder = seederServiceProvider.GetRequiredService<Seeder>();
await seeder.SeedAsync();
```

## Accessing Services in Seeders

The `Seeder` class uses constructor injection to access dependencies. You can access services through the `serviceProvider` parameter:

```csharp
public partial class Seeder
{
    private ApplicationDbContext _db = serviceProvider.GetRequiredService<ApplicationDbContext>();

    private async Task SeedDevelopmentContentAsync()
    {
        // Access other services as needed
        var notificationService = serviceProvider.GetRequiredService<INotificationService>();

        // Your seeding logic
    }
}
```

## Best Practices

### Do's

- **Keep it idempotent** - Check if data exists before creating it
- **Use transactions** - Wrap complex seeding in database transactions
- **Log your actions** - Use the injected `ILogger` to track seeding progress
- **Version your seed data** - Consider adding version checks for complex migrations
- **Test thoroughly** - Run your seeders multiple times to ensure idempotency
- **Use environment methods** - Add environment-specific logic directly to the appropriate method
- **Use Seeder.Local.cs** - Keep developer-specific seeding separate and untracked

### Don'ts

- **Don't assume order** - Roles are processed alphabetically, not in definition order
- **Don't seed sensitive data** - Use configuration for passwords and secrets
- **Don't commit Seeder.Local.cs** - Add it to `.gitignore` to keep it local
- **Don't skip error handling** - Seeding failures should stop application startup
- **Don't create too much data** - Large seed operations slow down startup

## Example: Seeding Related Entities

Here's an example of seeding complex related data in a development environment:

```csharp
private async Task SeedDevelopmentContentAsync()
{
    logger.LogInformation("Seeding Development environment content");

    // Reference existing seeded users by email
    // Note: Use appsettings.json SeededUsers configuration to create test users
    var admin = await _db.Users.FirstOrDefaultAsync(u => u.Email == "admin@lightnap.azurewebsites.net");
    var user1 = await _db.Users.FirstOrDefaultAsync(u => u.Email == "user1@site.com");

    if (admin == null || user1 == null)
    {
        logger.LogWarning("Test users not found. Configure them in appsettings.json SeededUsers.");
        return;
    }

    // Seed sample blog posts
    if (!_db.BlogPosts.Any())
    {
        _db.BlogPosts.AddRange(
            new BlogPost
            {
                Title = "Welcome Post",
                Content = "Welcome to our site!",
                AuthorId = admin.Id,
                CreatedAt = DateTime.UtcNow
            },
            new BlogPost
            {
                Title = "User Post",
                Content = "My first post!",
                AuthorId = user1.Id,
                CreatedAt = DateTime.UtcNow.AddHours(-1)
            }
        );
        await _db.SaveChangesAsync();
    }

    // Seed sample application-specific data
    if (!_db.Categories.Any())
    {
        _db.Categories.AddRange(
            new Category { Name = "Technology", Slug = "technology" },
            new Category { Name = "Science", Slug = "science" }
        );
        await _db.SaveChangesAsync();
    }

    logger.LogInformation("Seeded Development environment content");
}
```

{: .important }
**User Seeding Best Practice:** Prefer using the `SeededUsers` configuration in `appsettings.json` to create test users, including administrators and users for specific roles. The environment seeder should reference existing users rather than creating them. Additionally, note that LightNap automatically notifies administrators when new users register through the normal registration process, so you don't need to manually create these notifications in your seeder.

## Troubleshooting

### Seeding Fails Silently

Check the application logs during startup. Seeding errors are logged at the `Error` level and will cause the application to throw an exception.

### Data Not Appearing

Ensure that:

1. The seeding code is actually being executed (add log statements)
2. Database transactions are being committed (`SaveChangesAsync()`)
3. The idempotency checks aren't preventing creation

### Environment Seeder Not Running

Verify that:

1. The environment name matches exactly (case-sensitive)
2. The appropriate method is implemented in the `Seeder` class
3. The hosting environment is configured correctly in `launchSettings.json` or deployment settings

### Performance Issues

If seeding takes too long:

1. Reduce the amount of data being seeded
2. Use `AddRange()` instead of multiple `Add()` calls
3. Consider lazy initialization instead of startup seeding
4. Disable automatic migrations if not needed

## Related Topics

- [Seeding Users](../getting-started/seeding-users) - Detailed user seeding configuration
- [Application Configuration](../getting-started/application-configuration) - Overall configuration reference
- [Database Providers](../getting-started/database-providers) - Database setup and migrations
- [Adding Entities](./adding-entities) - Creating new database entities