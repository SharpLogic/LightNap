---
title: Security & Authorization
layout: home
parent: Development Guide
nav_order: 400
---

# {{ page.title }}

Guides for implementing authentication, authorization, and security features in LightNap.

## Available Guides

### [Rate Limiting](./rate-limiting)

Configure and understand rate limiting to protect your API from abuse and ensure fair usage.

### [CAPTCHA Verification](./captcha)

Protect public-write endpoints from bots. Pick from NoOp (dev), Cloudflare Turnstile, Google reCAPTCHA v2, or reCAPTCHA v3 via configuration and opt endpoints in with the `[ValidateCaptcha]` filter.

### [Working With Roles](./working-with-roles)

Add and manage application roles for authorization. Learn how to define roles, apply them to endpoints, and use them in the frontend.

### [Working With Custom Claims](./working-with-custom-claims)

Implement fine-grained permissions using ASP.NET claims. This guide covers claim-based authorization for dynamic, record-level security.
