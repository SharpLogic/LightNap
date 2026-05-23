---
title: Deployment & CI/CD
description: Overview of deployment and CI/CD workflows
layout: home
nav_order: 400
---

# {{ page.title }}

LightNap includes pre-configured GitHub Actions workflows for continuous integration, deployment, and documentation publishing. These workflows are disabled by default and can be activated by setting specific repository variables.

## CI/CD Workflows

### [Build, Test, and Publish](./cicd-workflows/build-test-publish)

The foundation CI workflow that builds both the .NET backend and Angular frontend, runs all tests, and packages the application as an artifact for deployment. This workflow triggers on commits to the `main` branch that modify the `src` folder.

**Activation**: Set repository variable `RUN_BUILD_TEST_PUBLISH` to `true`.

### [Deploy to Azure App Service](./cicd-workflows/deploy-to-azure)

Automatically deploy LightNap to Azure App Service after a successful build. This workflow takes the artifact from the Build, Test, and Publish workflow and deploys it to your Azure environment.

**Activation**:

1. Set repository variable `RUN_DEPLOY_TO_AZURE_APP_SERVICE` to `true`
2. Set repository secret `AZURE_APP_SERVICE_NAME` with your app service name
3. Set repository secret `AZURE_WEBAPP_PUBLISH_PROFILE` with your publish profile

## Documentation Workflows

### [Deploy to GitHub Pages](./documentation-workflows/github-pages)

Automatically build and deploy this Jekyll-based documentation site to GitHub Pages whenever changes are committed to the `docs` folder on the `main` branch.

**Activation**:

1. Enable GitHub Pages for your repository using "GitHub Actions" as the source
2. Set repository variable `RUN_BUILD_AND_DEPLOY_DOCS` to `true`

### [Automated Documentation Agent](./documentation-workflows/automated-docs-agent)

An AI-powered agent that analyzes code changes and proposes documentation updates. After merges to main that modify source code, the agent evaluates what documentation needs to be added, updated, or removed and creates a pull request with the proposed changes.

**Activation**:

1. Create an API key and add it as repository secret
   - `OPENAI_API_KEY` for OpenAI
   - `ANTHROPIC_API_KEY` for Anthropic
2. Set repository variable `RUN_DOCS_AGENT` to `true`

## Testing in CI/CD

### [Testing in CI/CD](./testing-in-cicd)

Learn how tests are integrated into the CI/CD pipeline, including running unit and E2E tests, generating coverage reports, and handling test failures.

## Workflow Architecture

The workflows follow a staged approach:

```mermaid
graph LR
    A[Code Push] --> B[Build, Test, Publish]
    B --> C{Tests Pass?}
    C -->|Yes| D[Create Artifact]
    C -->|No| E[Notify Failure]
    D --> F[Deploy to Azure]

    G[Docs Push] --> H[Build Docs]
    H --> I[Deploy to GitHub Pages]
```

## Getting Started with Workflows

1. **Start with Build, Test, Publish** - Ensures your code builds and tests pass on every commit
2. **Add Azure Deployment** - Automate deployments after builds succeed
3. **Enable Documentation** - Keep your docs up-to-date automatically

## Best Practices

- **Test Locally First** - Ensure builds and tests work locally before enabling CI/CD
- **Secure Your Secrets** - Always use GitHub secrets for sensitive data like connection strings and publish profiles
- **Review Workflow Logs** - Check the Actions tab in GitHub for detailed execution logs
- **Customize As Needed** - These workflows are templates; modify them to fit your deployment needs

## Environment-Specific Configuration Overrides

Both `LightNap.WebApi` and `LightNap.MaintenanceService` use the standard `Host.CreateApplicationBuilder` pipeline (via `WebApplication.CreateBuilder` for WebApi and an explicit `ConfigureAppConfiguration` for the MaintenanceService), which automatically loads `appsettings.{Environment}.json` on top of `appsettings.json` when a file with the current environment name is present in the output directory. Environment variables are layered last and win over both JSON files.

To override settings for a production deployment without committing secrets:

1. Drop a gitignored `appsettings.Production.json` into the target project (e.g. `src/LightNap.WebApi/` or `src/LightNap.MaintenanceService/`) containing only the keys that differ — for example a different `Database:Provider`, a Key Vault URL for `DataProtection:Azure:KeyVaultUrl`, or production SMTP credentials.
2. Set `ASPNETCORE_ENVIRONMENT=Production` for the WebApi process and `DOTNET_ENVIRONMENT=Production` for the MaintenanceService process so each host picks up the matching `appsettings.Production.json`.
3. For values that should never live in a file even on the production host (passwords, connection strings, signing keys), prefer environment variables — they have the highest precedence and bypass the file entirely. Use the standard double-underscore syntax to address nested keys (`ConnectionStrings__DefaultConnection`, `DataProtection__Azure__KeyVaultUrl`).

The repo ships with `appsettings.E2e.json` as the only environment override checked in (used by the end-to-end test profile). All other `appsettings.*.json` files are gitignored.
