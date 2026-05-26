---
title: Administrative Audit Log
layout: home
parent: Concepts
nav_order: 1000
---

# {{ page.title }}

Admin surfaces in any non-trivial app need an audit trail: who deactivated the user, who changed the role, who modified the CMS entry. LightNap ships a generic primitive so every admin controller doesn't reinvent its own audit pattern.

## What you get

| Piece                              | Purpose                                                                                   |
|------------------------------------|-------------------------------------------------------------------------------------------|
| `AdminAuditLog` entity             | Stores one row per recorded action.                                                       |
| `IAuditLogger` / `AuditLogger`     | Domain service that writes audit rows. Actor is resolved from `IUserContext`.             |
| `[AuditLog("action.name")]`        | Declarative filter that records the action arguments on successful execution.             |
| `PurgeExpiredAuditLogsMaintenanceTask` | Scheduled task that drains entries older than the configured retention window.        |
| `AuditLogRetentionSettings`        | Configures retention in days (default 365).                                                |

## Recording an action

For the common case where the action arguments are a good enough "after" snapshot:

```csharp
[HttpPost("users/{id}/deactivate")]
[AuditLog("user.deactivate")]
public async Task<IActionResult> Deactivate(string id) { ... }
```

The filter records `{ id }` as the after-snapshot on any 2xx (non-BadRequest) result.

For richer audit (before / after snapshots, different action name per code path), inject `IAuditLogger` directly:

```csharp
public async Task<IActionResult> AssignRole(string userId, string role)
{
    var before = await db.UserRoles.Where(...).Select(...).ToListAsync();
    await usersService.AssignRoleAsync(userId, role);
    var after = await db.UserRoles.Where(...).Select(...).ToListAsync();

    await auditLogger.WriteAsync(
        action: "user.role.assign",
        targetType: "User",
        targetId: userId,
        before: before,
        after: after);

    return Ok();
}
```

## Failure handling

`[AuditLog]` does not record an entry when:

- The action threw an unhandled exception.
- The action returned a `BadRequestObjectResult` (the convention for validation failure).

Other 4xx responses do still record an entry — they generally represent meaningful administrative actions (e.g., a 404 from an admin lookup is worth knowing about).

## Retention

The `PurgeExpiredAuditLogsMaintenanceTask` is registered in `LightNap.MaintenanceService` alongside the existing refresh-token and user-settings purges. It runs on the same external schedule (cron, Azure WebJob, GitHub Action) and deletes entries with `CreatedAt < UtcNow - RetentionDays`.

```jsonc
"Audit": {
  "Retention": {
    "RetentionDays": 365
  }
}
```

Defaults to 365 days when omitted.

## Why "actor" and not "user"

The audit row records `ActorId`, not `UserId`, because the actor may be a system context (`"system"` for seeders and maintenance jobs) or — when [anonymous visitor tracking](./anonymous-visitor) is enabled — an anonymous visitor identifier. Consumer code calls `IUserContext.GetActorId()` to obtain the right value without branching on actor kind. Until the visitor middleware is registered, only authenticated and system actors produce entries.

## Database support

Migrations are included for SQLite and SQL Server (the two relational providers LightNap ships with). The `AdminAuditLogs` table is plain — single primary key, no indexes by default. Add indexes via a follow-on migration if you query the audit log on a hot path (e.g., for an admin-facing audit viewer).
