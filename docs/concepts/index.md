---
title: Concepts
layout: home
nav_order: 250
---

# {{ page.title }}

This section explains the core concepts and architectural patterns used throughout LightNap. Each page covers what something is and why it exists; for step-by-step "how do I wire this up" guides, see the [Development Guide](../development-guide).

## Architecture & Design

### [Solution & Project Structure](./project-structure)

Understand how LightNap is organized: the .NET backend projects, the Angular frontend, and the data flow pattern that connects them. The foundation for everything else.

### [API Response Model](./api-response-model)

LightNap's standardized REST response shape, error handling, and how the Angular client automatically processes responses.

## Identity & Sessions

### [Authentication & Tokens](./authentication)

The JWT-plus-refresh-token model, session lifetime, and how the framework verifies and issues credentials.

### [Devices](./devices)

Per-device session tracking — what's recorded, how stale sessions age out, and how users see and revoke them.

### [Anonymous Visitor Tracking](./anonymous-visitor)

Opt-in middleware that mints a per-browser visitor cookie so unauthenticated users have a stable identifier for audit, anonymous UGC, and per-visitor rate limiting that survives NAT and VPN hops.

## Data Persistence

### [JSON Property Storage](./json-storage)

`[StoredAsJson]` persists strongly-typed POCO properties as JSON on entity columns with no per-entity wiring. Behavior is identical across SQLite, SQL Server, and InMemory.

## Infrastructure

### [HTTP Resilience](./http-resilience)

Wire outbound `HttpClient`s with retry, timeouts, circuit breaker, and a concurrency limiter in one call. Use for every outbound call to a service LightNap doesn't own.

## Operations

### [Health Check Endpoints](./health-checks)

`/health/live` and `/health/ready` for container orchestrators and uptime monitors. Readiness covers the database and Redis in distributed mode; liveness is dependency-free.

## Background Work

### [Periodic and Background Tasks](./periodic-tasks)

When to use the one-shot `LightNap.MaintenanceService` model vs an in-process `IHostedService` with `PeriodicTimer`. Covers when an external scheduler like Quartz or Hangfire is actually warranted.

## Compliance & Audit

### [Administrative Audit Log](./audit-log)

`AdminAuditLog` entity, `IAuditLogger` service, and `[AuditLog]` filter make recording who-did-what on admin endpoints a one-line addition. Retention is configurable; a scheduled maintenance task purges expired entries.

## Core Features

### [Content Management System](./content-management)

LightNap's built-in CMS — zones, pages, multilingual content, access control, and frontend integration.

### [Breadcrumb Navigation](./breadcrumb-navigation)

Breadcrumb generation from route configuration, including dynamic content from route parameters.

## Testing Fundamentals

### [Testing Fundamentals](./testing-fundamentals)

Introduction to testing concepts, frameworks, and architecture in LightNap.

### [Unit Testing Guide](../development-guide/testing/unit-testing)

Detailed guide for unit testing Angular components and services with Karma and Jasmine.

### [E2E Testing Guide](../development-guide/testing/e2e-testing)

Complete guide for end-to-end testing with Cypress to validate user workflows.

## Getting Started

New to LightNap? Start with:

1. **[Solution & Project Structure](./project-structure)** — get oriented with the codebase.
2. **[Authentication & Tokens](./authentication)** — understand the security model.
3. **[API Response Model](./api-response-model)** — learn the API patterns.

Then explore the [Development Guide](../development-guide) for practical implementation guides.
