---
title: Configuring Application Settings
layout: home
parent: Getting Started
nav_order: 300
---

# {{ page.title }}

Application settings need to be [configured in `appsettings.json`](./application-configuration) or your deployment host.

| File                             | Purpose                                                                                                                                                                                                                                                                                           |
| :------------------------------- | :------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `AutomaticallyApplyEfMigrations` | `true` to automatically apply Entity Framework migrations. If this is set to false, EF migrations must be manually applied. This setting is ignored for non-relational databases like the in-memory provider.                                                                                     |
| `LogOutInactiveDeviceDays`       | The number of days of inactivity (no contact) before a device is logged out.                                                                                                                                                                                                                      |
| `RequireEmailVerification`       | `true` to require users to verify their emails before they log in.                                                                                                                                                                                                                                |
| `RequireTwoFactorForNewUsers`    | `true` to require two-factor email authentication for new users. It does not change existing users.                                                                                                                                                                                               |
| `SiteUrlRootForEmails`           | The root URL used in emails. This should be the base URL where the Angular app is hosted from (e.g., `https://localhost:4200/` in development).                                                                                                                                                   |
| `UseSameSiteStrictCookies`       | `true` to use SameSite strict cookies. Set this to `false` if debugging the frontend from a URL that doesn't match the base URL hosting the Web API. For example, when connecting a frontend hosted at `http://localhost:4200/` against a backend hosted at `https://lightnap.azurewebsites.net`. |
