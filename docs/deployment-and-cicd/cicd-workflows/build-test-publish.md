---
title: Build, Test, and Publish
layout: home
parent: CI/CD Workflows
nav_order: 10
---

# {{ page.title }}

The `build-test-publish.yaml` runs on a `main` branch commit to the `src` folder. It builds both the frontend and backend, runs their tests, and packages them up as an artifact to be consumed by one of the deployment workflows.

It is disabled by default.

## Usage

1. Create a repo variable `RUN_BUILD_TEST_PUBLISH` and set it to `true`.
