---
title: Configuring Application Settings
layout: home
parent: Application Configuration
nav_order: 300
---

# {{ page.title }}

Application settings need to be [configured in `appsettings.json`](./index) or your deployment host.

## Database Settings

| Setting                          | Purpose                                                                                                                                                                                                                                                                                           |
| -------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Database:Provider`              | The database provider to use. Valid values are `"SqlServer"`, `"Sqlite"`, or `"InMemory"`.                                                                                                                                                                                                        |
| `Database:AutomaticallyApplyEfMigrations` | `true` to automatically apply Entity Framework migrations. If this is set to false, EF migrations must be manually applied. This setting is ignored for non-relational databases like the in-memory provider.                                                                                     |

## Authentication Settings

| Setting                          | Purpose                                                                                                                                                                                                                                                                                           |
| -------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Authentication:LogOutInactiveDeviceDays` | The number of days of inactivity (no contact) before a device is logged out.                                                                                                                                                                                                                      |
| `Authentication:RequireEmailVerification` | `true` to require users to verify their emails before they log in.                                                                                                                                                                                                                                |
| `Authentication:RequireTwoFactorForNewUsers` | `true` to require two-factor email authentication for new users. It does not change existing users.                                                                                                                                                                                               |

## Rate Limiting

See the options for configuring [rate limiting](../configuring-rate-limiting).
