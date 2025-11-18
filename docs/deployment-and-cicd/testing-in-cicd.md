---
title: Testing in CI/CD
layout: home
parent: Deployment & CI/CD
nav_order: 300
---

# {{ page.title }}

This section covers how testing is integrated into LightNap's CI/CD pipelines, ensuring quality and reliability in automated deployments.

## Running Tests in Pipelines

### Unit Test Execution

```yaml
- name: Run Unit Tests
  run: npm run test:ci
  working-directory: src/lightnap-ng
```

- Uses headless Chrome for CI environments
- Generates coverage reports
- Fails build if coverage thresholds not met

### E2E Test Execution

```yaml
- name: Run E2E Tests
  run: npm run e2e:ci
  working-directory: src/lightnap-ng
```

- Runs in headless mode
- Supports parallel execution
- Records results to Cypress Dashboard
- Runs against a live backend by default (use `e2e:mocks` for mocked responses)
- Requires special consideration for having both backend and frontend servers already running in parallel

## Coverage Reporting

### Coverage Collection

Tests generate multiple coverage formats:

- **HTML**: Interactive reports in `coverage/lightnap-ng/index.html`
- **LCOV**: For CI tools like Codecov
- **JSON**: For custom processing

### CI Integration

```yaml
- name: Upload Coverage
  uses: codecov/codecov-action@v3
  with:
    file: src/lightnap-ng/coverage/lcov.info
    flags: unit-tests
```

## Handling Test Failures

### Failure Analysis

- **Screenshots**: Automatic capture on E2E failures
- **Videos**: Optional video recording for debugging
- **Logs**: Detailed error output in CI logs

### Retry Strategies

```yaml
- name: Run Tests with Retry
  uses: nick-invision/retry@v2
  with:
    command: npm run test:ci
    timeout_minutes: 10
    max_attempts: 3
```

### Flaky Test Management

- Identify and quarantine flaky tests
- Use Cypress's retry mechanism for transient failures
- Monitor test stability over time

## Parallel Test Execution

### E2E Parallelization

```bash
# package.json
"e2e:ci": "cypress run --record --parallel"
```

- Distributes tests across multiple CI containers
- Requires Cypress Dashboard for coordination
- Reduces total execution time

### Unit Test Parallelization

```yaml
- name: Run Unit Tests
  run: npm run test:ci -- --parallel
```

## Performance Monitoring

### Test Execution Times

- Track test duration trends
- Set performance budgets for test suites
- Alert on significant slowdowns

### Coverage Trends

- Monitor coverage over time
- Prevent coverage regression
- Identify under-tested areas

## Best Practices for CI/CD Testing

### Test Selection

- **Smoke Tests**: Quick validation after deployment
- **Regression Tests**: Full suite on main branch
- **Feature Tests**: Targeted tests for feature branches

### Environment Consistency

- Use same Node.js and browser versions
- Consistent viewport sizes
- Isolated test databases

### Failure Handling

- Don't fail builds on flaky tests
- Provide clear failure reports
- Enable debugging for failed runs

### Security

- Don't expose test credentials in logs
- Use secret management for API keys
- Sanitize sensitive data in reports
