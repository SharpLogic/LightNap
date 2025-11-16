---
title: CI/CD Workflows
layout: home
parent: Deployment & CI/CD
nav_order: 100
---

# {{ page.title }}

Automated workflows for building, testing, and deploying LightNap applications.

## Available Workflows

### [Build, Test, and Publish](./build-test-publish)

The foundation CI workflow that builds both the .NET backend and Angular frontend, runs all tests, and packages the application as an artifact for deployment.

### [Deploy to Azure](./deploy-to-azure)

Deploys the packaged application to Azure App Service using the artifacts created by the build workflow.
