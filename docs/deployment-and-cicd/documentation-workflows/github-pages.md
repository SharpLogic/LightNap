---
title: Deploy to GitHub Pages
layout: home
parent: Documentation Workflows
nav_order: 20
---

# {{ page.title }}

The `github-pages.yaml` runs on a `main` branch commit to the `docs` folder. This workflow automates the deployment of a
Jekyll-based documentation site to GitHub Pages. The site is based on [Just the Docs](https://just-the-docs.com/).

It is disabled by default.

## Usage

1. Set up GitHub Pages for your repo using the `GitHub Actions` source
2. Create a repo variable `RUN_BUILD_AND_DEPLOY_DOCS` and set it to `true`.
