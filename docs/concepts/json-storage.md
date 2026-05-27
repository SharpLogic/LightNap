---
title: JSON Property Storage
layout: home
parent: Concepts
nav_order: 600
---

# {{ page.title }}

LightNap provides a small primitive for persisting strongly-typed POCO properties as JSON in the database without per-entity wiring of `HasConversion(...)` and without depending on provider-specific queryable JSON features. The result is one attribute per property and identical behavior across SQLite, SQL Server, and the InMemory provider.

## When to use it

Mark a property with `[StoredAsJson]` when the value is an **opaque payload** — something the application reads and writes as a whole, but never queries by:

- Per-user preference blobs that the UI round-trips.
- Audit details captured at event time.
- Configuration snapshots stored alongside a record.
- Structured payloads on log entries.

## When not to use it

If you ever want to filter, sort, or index by a field inside the payload, **promote it to its own column**. The convention is intentionally opaque to make this trade-off explicit:

> If you'd index a JSON path, it's a column.

LightNap deliberately does not support provider-native queryable JSON (`OPENJSON`, `json_extract`, etc.) for this attribute, because those features are not available on the InMemory provider used in tests. Mixing opaque-blob storage with queryable JSON in the same property tends to leak provider-specific behavior into the domain model.

## How it works

Two pieces:

1. `[StoredAsJson]` on a reference-type entity property.
2. `modelBuilder.ApplyJsonValueConverters()` inside `OnModelCreating`, which scans every configured entity for marked properties and wires `JsonValueConverter<T>` to each.

`JsonValueConverter<T>` stores values as `string` and uses `System.Text.Json` with camelCase naming and null-skipping by default. To override the serializer behavior on a specific property, register the converter manually via `HasConversion(new JsonValueConverter<T>(customOptions))` instead of using the attribute.

## Provider behavior

| Provider     | Column type      | Notes                                         |
|--------------|------------------|-----------------------------------------------|
| SQL Server   | `nvarchar(max)`  | Stored as text. Not queried via `OPENJSON`.   |
| SQLite       | `TEXT`           | Stored as text.                               |
| InMemory     | `string`         | Round-trips through the same converter.       |

Because the storage type is always `string`, behavior is identical across providers.
