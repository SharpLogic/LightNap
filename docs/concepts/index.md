---
title: Concepts
layout: home
nav_order: 250
---

# {{ page.title }}

This section explains the core concepts and architectural patterns used throughout LightNap. Understanding these concepts will help you work more effectively with the framework and make informed decisions when extending it.

## Architecture & Design

### [Solution & Project Structure](./project-structure)

Understand how LightNap is organized, including the .NET backend projects, Angular frontend structure, and the data flow pattern that connects them. This is the foundation for understanding how all the pieces fit together.

### [API Response Model](./api-response-model)

Learn about LightNap's standardized REST API response format, including how errors are handled, HTTP status codes are used, and how the frontend automatically processes API responses.

## Infrastructure

### [HTTP Resilience](./http-resilience)

Wire outbound `HttpClient`s with retry, timeouts, circuit breaker, and a concurrency limiter in one line. Use for every outbound call to a service LightNap doesn't own.

## Data Persistence

### [JSON Property Storage](./json-storage)

Learn how `[StoredAsJson]` lets you persist strongly-typed POCO properties as JSON on entity columns with one attribute and no per-entity wiring. Behavior is identical across SQLite, SQL Server, and InMemory.

## Background Work

### [Periodic and Background Tasks](./periodic-tasks)

Pick between the one-shot `LightNap.MaintenanceService` model (nightly cron-style maintenance) and an in-process `IHostedService` with `PeriodicTimer` (continuous work while the WebApi is up). Covers when an external scheduler like Quartz or Hangfire is actually warranted.

## Operations

### [Health Check Endpoints](./health-checks)

`/health/live` and `/health/ready` for container orchestrators and uptime monitors. Readiness covers the database and Redis (in distributed mode); liveness is dependency-free.

## Identity & Visitors

### [Anonymous Visitor Tracking](./anonymous-visitor)

Opt-in middleware that mints and reads a per-browser visitor cookie so unauthenticated users have a stable identifier for audit, anonymous UGC attribution, and per-visitor rate limiting that survives NAT and VPN hops.

## Compliance & Audit

### [Administrative Audit Log](./audit-log)

`AdminAuditLog` entity plus `IAuditLogger` and `[AuditLog]` filter make it a one-line addition to record who did what on any admin endpoint. Retention is configurable, with a scheduled maintenance task that purges expired entries.

## Core Features

### [Content Management System](./content-management)

Explore LightNap's built-in CMS that enables administrators and content editors to create, manage, and publish multilingual static content. Learn about pages, zones, access control, and frontend integration.

### [Breadcrumb Navigation](./breadcrumb-navigation)

Learn about LightNap's breadcrumb navigation system that automatically generates navigation trails based on route configuration, supporting both static labels and dynamic content from route parameters.

## Testing Fundamentals

### [Testing Fundamentals](./testing-fundamentals)

Introduction to testing concepts, frameworks, and architecture in LightNap.

### [Unit Testing Guide](../development-guide/testing/unit-testing)

Detailed guide for unit testing Angular components and services with Karma and Jasmine.

### [E2E Testing Guide](../development-guide/testing/e2e-testing)

Complete guide for end-to-end testing with Cypress to validate user workflows.

## Getting Started

New to LightNap? Start with:

1. **[Solution & Project Structure](./project-structure)** - Get oriented with the codebase
2. **[Authentication & Tokens](./authentication)** - Understand the security model
3. **[API Response Model](./api-response-model)** - Learn the API patterns

Then explore the [Development Guide](../development-guide) section for practical implementation guides.
 
 
