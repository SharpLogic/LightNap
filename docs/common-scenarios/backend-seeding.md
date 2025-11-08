---
title: Backend Seeding
layout: home
parent: Common Scenarios
nav_order: 150
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

Calls the optional `SeedEnvironmentContent()` partial method, which can be implemented in environment-specific files like `Seeder.Development.cs`.

## Environment-Specific Seeding

One of the key features of LightNap's seeding system is support for environment-specific seeding through **partial classes**. This allows you to include development data without risking it being deployed to production.

### How It Works

The `Seeder` class is declared as `partial` and includes an optional partial method:

```csharp
partial void SeedEnvironmentContent();
```

You can implement this method in a separate file with a conditional build symbol:

#### Seeder.Development.cs

```csharp
namespace LightNap.WebApi.Configuration
{
    public partial class Seeder
    {
        private ApplicationDbContext _db = serviceProvider.GetRequiredService<ApplicationDbContext>();

        partial void SeedEnvironmentContent()
        {
            this.SeedEnvironmentContentInternalAsync().Wait();
        }

        private async Task SeedEnvironmentContentInternalAsync()
        {
            // Seed sample data for development/testing
            if (!_db.SomeEntities.Any())
            {
                _db.SomeEntities.Add(new SomeEntity { Name = "Test Data" });
                await _db.SaveChangesAsync();
            }
        }
    }
}
```

### Environment File Examples

You can create different seeder files for each environment:

- **Seeder.Development.cs** - Test users, sample data, mock content
- **Seeder.Staging.cs** - Staging-specific data that mimics production
- **Seeder.Production.cs** - Production-specific initialization (use sparingly)

{: .note }
Only include the appropriate seeder file in your build configuration for each environment. Never deploy development seeders to production.

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

## Accessing Services in Environment Seeders

The `Seeder` class uses constructor injection to access dependencies. In your environment-specific seeder, you can access services through the `serviceProvider` parameter:

```csharp
public partial class Seeder
{
    private ApplicationDbContext _db = serviceProvider.GetRequiredService<ApplicationDbContext>();

    partial void SeedEnvironmentContent()
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

### Don'ts

- **Don't assume order** - Roles are processed alphabetically, not in definition order
- **Don't seed sensitive data** - Use configuration for passwords and secrets
- **Don't deploy development seeders** - Use build conditions to exclude them
- **Don't skip error handling** - Seeding failures should stop application startup
- **Don't create too much data** - Large seed operations slow down startup

## Example: Seeding Related Entities

Here's an example of seeding complex related data in a development environment:

```csharp
private async Task SeedEnvironmentContentInternalAsync()
{
    // Reference existing seeded users by email
    // Note: Use appsettings.json SeededUsers configuration to create test users
    // rather than calling GetOrCreateUserAsync() here. See the Seeding Users guide.
    var admin = await _db.Users.FirstOrDefaultAsync(u => u.Email == "admin@admin.com");
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

1. The file is included in the build configuration
2. The partial method signature matches exactly
3. Build conditions are correct for your environment

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
