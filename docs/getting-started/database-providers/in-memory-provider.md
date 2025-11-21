---
title: In-Memory Provider
parent: Database Providers
layout: home
nav_order: 30
---

# {{ page.title }}

LightNap supports an in-memory database provider for development, testing, and rapid prototyping.

## Configuration

Update `appsettings.json`:

```json
{
  "Database": {
    "Provider": "InMemory"
  }
}
```

No connection string is required.

## Usage

- Data is lost when the application restarts
- Perfect for unit testing and integration tests
- Fast startup with no external dependencies
- Useful for development and prototyping

## Best Practices

- Use for automated testing
- Ideal for CI/CD pipelines
- Great for feature development before database design
- Not suitable for production or data persistence needs
