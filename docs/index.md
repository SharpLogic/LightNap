---
title: LightNap
layout: home
nav_order: 100
---

# {{ page.title }}

# Full-stack starter kit for modern SPAs 🚀

LightNap (**light**weight .**N**ET 10/**A**ngular/**P**rimeNG) bundles a pragmatic set of tools and patterns to build production-ready single page applications fast. If you want authentication, notifications, scaffolding and a modern front-end experience without wiring everything from scratch, LightNap is for you.

> Quick highlights: Lightweight starter, secure defaults, extensible scaffolding, and helpful CI/CD docs — ready to jump-start your project.

---

## Quick start — Try LightNap in 3 steps

1. Clone the repository

   ```bash
   git clone https://github.com/sharplogic/LightNap.git
   cd LightNap
   ```

2. Run the backend

   ```bash
   dotnet run --project LightNap.WebApi
   ```

3. Start the Angular frontend (in a separate terminal)

   ```bash
   cd src/lightnap-ng
   npm install
   ng serve
   ```

Tip: The demo site runs at [https://lightnap.azurewebsites.net](https://lightnap.azurewebsites.net) — visit it to see features in action. The application comes with three pre-seeded user accounts for testing: Administrator (`Admin@lightnap.azurewebsites.net` / `P@ssw0rd`), Content Editor (`ContentEditor@lightnap.azurewebsites.net` / `P@ssw0rd`), and Regular User (`RegularUser@lightnap.azurewebsites.net` / `P@ssw0rd`).

---

## Why choose LightNap? ✅

- Works with .NET 10 and Angular 20 UI (PrimeNG + Tailwind) out-of-the-box.
- Built-in auth with ASP.NET Identity + JWT for secure session handling.
- Flexible persistence: SQL Server, SQLite, or in-memory for quick experiments.
- Distributed caching with Redis and real-time notifications with SignalR.
- Production-ready patterns (scaffolding, seed data, notification system, and more).

---

## Key features — At a glance ✨

- **Authentication & Authorization:** Roles, claims, and user management ready in the admin UI.
- **APIs & OpenAPI (Swagger):** Quickly inspect and test backend endpoints during development.
- **Notifications:** Real-time in-app notifications with SignalR and a backend notification service for system alerts.
- **Scaffolder:** Auto-generate backend entities and front-end UI from a single entity definition.
- **Maintenance Service:** Background task processing with Azure WebJobs support for scheduled maintenance.
- **E2E & CI/CD:** Cypress-based end-to-end tests with guidance for workflows and GitHub Actions.

---

## Learn more

- Getting started guide: [Building & running the project](./getting-started/building-and-running)
- Scaffolding & development: [Scaffolding docs](./development-guide/data-persistence/scaffolding)
- Security and roles: [Security & authorization](./development-guide/security-authorization)
- Testing & CI/CD: [Testing in CI/CD](./deployment-and-cicd/testing-in-cicd)

---

## Contribute & Community

We welcome PRs and contributions — check the repo, open issues, or add documentation. Find sample walk-throughs on our YouTube channel and try the demo to get a feel for the code.

- Demo: [https://lightnap.azurewebsites.net](https://lightnap.azurewebsites.net)
- Docs: [https://lightnap.sharplogic.com](https://lightnap.sharplogic.com)
- YouTube: [https://www.youtube.com/@LightNap](https://www.youtube.com/@LightNap)
