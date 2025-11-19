---
title: Deploy to Azure App Service
layout: home
parent: CI/CD Workflows
nav_order: 20
---

# {{ page.title }}

The `deploy-to-azure.yaml` is configured to run after a successful [Build, Test, and Publish](./build-test-publish) workflow run.
It takes the build artifact and publishes it to an Azure App Service.

It is disabled by default.

## Usage

1. Set up your Azure environment (app service and database).
2. Create a repo secret `AZURE_APP_SERVICE_NAME` and set it to the name of your app service (like `my-app-service`).
3. Create a repo secret `AZURE_WEBAPP_PUBLISH_PROFILE` and paste in the contents of your
  [downloaded publish profile](https://learn.microsoft.com/en-us/visualstudio/azure/how-to-get-publish-profile-from-azure-app-service).
4. Create a repo variable `RUN_DEPLOY_TO_AZURE_APP_SERVICE` and set it to `true`.

## Scaling and Distributed Deployment

For high-availability deployments, you can scale your Azure App Service to multiple instances. When running multiple instances:

1. Enable distributed mode by setting `UseDistributedMode` to `true` in your application configuration.
2. Configure a Redis cache (Azure Cache for Redis) and set the connection string in your app service environment variables.
3. Ensure your database supports concurrent access, as migrations and seeding will be coordinated across instances.

This setup allows multiple backend instances to share cache and SignalR connections, providing a scalable and resilient deployment.
