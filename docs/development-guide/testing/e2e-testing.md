---
title: E2E Testing with Cypress
layout: home
parent: Testing
nav_order: 20
---

# {{ page.title }}

LightNap uses **Cypress** for end-to-end testing, enabling automated testing of complete user workflows from the browser perspective. This guide covers writing, configuring, and running E2E tests.

## Test Configuration

E2E tests are configured in `cypress.config.ts`:

```typescript
export default defineConfig({
  e2e: {
    baseUrl: 'http://localhost:4200',
    specPattern: 'cypress/e2e/**/*.cy.{js,jsx,ts,tsx}',
    supportFile: 'cypress/support/e2e.ts',
    viewportWidth: 1280,
    viewportHeight: 720,
    defaultCommandTimeout: 10000,
    requestTimeout: 15000,
    responseTimeout: 15000,
  }
})
```

Key settings:

- **baseUrl**: Application URL for tests
- **Timeouts**: Generous timeouts for reliable execution
- **Viewport**: Consistent screen size across tests

## Writing E2E Tests

### Basic Test Structure

```typescript
describe('Feature Name', () => {
  beforeEach(() => {
    cy.visit('/');
  });

  it('should perform user action', () => {
    // Test implementation
  });
});
```

### Common Patterns

#### Authentication Testing

```typescript
it('should login successfully', () => {
  cy.visit('/identity/login');
  cy.get('[data-cy="login-username"]').type('E2eRegularUser');
  cy.get('[data-cy="login-password"]').type('P@ssw0rd');
  cy.get('[data-cy="login-submit"]').click();

  cy.url().should('not.include', '/identity/login');
  cy.get('[data-cy="user-menu"]').should('be.visible');
});
```

#### Form Testing

```typescript
it('should validate form inputs', () => {
  cy.visit('/some-form');

  // Test required field
  cy.get('[data-cy="submit-btn"]').click();
  cy.get('[data-cy="error-message"]').should('contain', 'Required field');

  // Fill form and submit
  cy.get('[data-cy="name-input"]').type('Test Name');
  cy.get('[data-cy="submit-btn"]').click();
  cy.get('[data-cy="success-message"]').should('be.visible');
});
```

#### Navigation Testing

```typescript
it('should navigate between pages', () => {
  cy.visit('/');
  cy.get('[data-cy="nav-link"]').click();
  cy.url().should('include', '/target-page');
  cy.get('[data-cy="page-title"]').should('contain', 'Target Page');
});
```

## Custom Commands

LightNap provides reusable Cypress commands in `cypress/support/commands.ts`:

```typescript
// Authentication helpers
cy.logInRegularUser();
cy.logInAdministrator();
cy.logInContentEditor();
cy.logout();
cy.isLoggedIn();
cy.shouldBeLoggedIn();
cy.shouldBeLoggedOut();

// Mock setup
cy.setupContentMocks();
cy.setupAdminMocks();
```

### Using Custom Commands

```typescript
it('should allow admin to access restricted area', () => {
  cy.logInAdministrator();
  cy.visit('/admin/users');
  cy.get('[data-cy="manage-users-panel"]').should('be.visible');
});
```

## Running E2E Tests

### With Mocks

```bash
npm run e2e:mocks
```

Runs headless tests with REST API mocking enabled.

### Interactive Mode

```bash
npm run e2e:mocks:open
```

Opens Cypress Test Runner with mocks for interactive test development.

### Against Live Backend

To run tests against a live backend, you need to start both the frontend and backend servers.

#### Starting the Backend

Start the backend in E2E mode, which seeds test users and content:

```bash
npm run e2e:backend
```

This runs the backend with the `E2e` launch profile, which:

- Uses the E2E environment configuration
- Seeds test users with credentials:
  - **E2eRegularUser** / P@ssw0rd (regular user)
  - **E2eAdmin** / P@ssw0rd (administrator)
  - **E2eContentEditor** / P@ssw0rd (content editor)
- Seeds E2E-specific test content

#### Starting the Frontend

In a separate terminal, start the frontend development server:

```bash
npm run start
```

#### Running Tests

Once both servers are running, execute tests against the live backend:

```bash
# Headless mode
npm run e2e

# Interactive mode
npm run e2e:open
```

{: .note }
The E2E environment uses [backend seeding](../../development-guide/data-persistence/backend-seeding) to automatically configure test users and content. This ensures a consistent test environment across different machines and CI/CD pipelines.

### CI Mode

```bash
npm run e2e:ci
```

Runs tests in CI mode with recording and parallel execution for Cypress Dashboard. This task now runs against a live backend by default (no mocking) to get realistic end-to-end coverage.

If you prefer to run CI-style tests against mocked responses, use the `e2e:mocks` script or set the `useMocks` env variable explicitly in your CI pipeline. For example:

```bash
npx cross-env useMocks=true npm run e2e:ci
```

## Test Credentials

When running tests against a live backend, use these seeded credentials:

| User | Username | Password | Roles |
|------|----------|----------|-------|
| Regular User | E2eRegularUser | P@ssw0rd | - |
| Administrator | E2eAdmin | P@ssw0rd | Administrator |
| Content Editor | E2eContentEditor | P@ssw0rd | ContentEditor |

These credentials are automatically seeded when the backend runs in E2E mode.

{: .note }
The backend now includes an `appsettings.E2e.json` file that seeds a set of test users and simple content for E2E runs; this is consumed when you start the backend with the E2e launch profile (see `Documentation -> Getting Started -> Application Configuration`).

## Test Organization

### File Structure

```tree
cypress/
├── e2e/
│   ├── authentication.cy.ts
│   ├── content.cy.ts
│   ├── admin.cy.ts
│   └── ...
├── support/
│   ├── commands.ts
│   ├── e2e.ts
│   └── mock-api.ts
└── fixtures/
    └── example.json
```

### Test Categories

- **authentication.cy.ts**: Login, registration, password reset
- **content.cy.ts**: CMS functionality, page management
- **admin.cy.ts**: Administrative features, user management
- **profile.cy.ts**: User profile, settings, notifications

## Mocking and Fixtures

### API Mocking

Use `cypress/support/mock-api.ts` for API interception:

```typescript
cy.intercept('GET', '/api/users', { fixture: 'users.json' }).as('getUsers');
cy.visit('/users');
cy.wait('@getUsers');
```

### Environment-Based Mocking

```bash
# Run with mocks
npm run e2e:mocks:open
```

Configure mocks in `cypress.config.ts`:

```typescript
env: {
  useMocks: false
}
```

### Mock vs Live Backend

The custom commands automatically handle differences between mocked and live backends:

- **With mocks**: Uses Cypress sessions to cache login state for faster test execution
- **Live backend**: Skips session caching to properly handle refresh token cookie updates

## Best Practices

### Test Data Management

- Use fixtures for static data
- Seed test data programmatically when needed
- Clean up data between tests
- Use the E2E environment for consistent test data

### Selector Strategy

- Prefer `data-cy` attributes over CSS selectors
- Avoid brittle selectors like `.class:nth-child(3)`
- Use descriptive data attributes

### Test Isolation

- Each test should be independent
- Use `beforeEach` for common setup
- Avoid test interdependencies

### Performance

- Use API mocking for faster execution
- Group related tests in the same spec file
- Parallel execution in CI

### Debugging

- Use `cy.debug()` and `cy.pause()` during development
- Check screenshots/videos on failures
- Use `cy.log()` for debugging output

## Troubleshooting

### Common Issues

#### Tests fail intermittently

- Increase timeouts in `cypress.config.ts`
- Use `cy.wait()` for async operations
- Check for race conditions

#### Element not found

- Verify `data-cy` attributes exist
- Check if element is rendered after navigation
- Use `cy.get()` with timeout options

#### API calls not intercepted

- Ensure intercept is set before the action
- Check URL patterns match exactly
- Use `cy.intercept()` with wildcards

#### Slow test execution

- Enable API mocking
- Reduce viewport size if not needed
- Use `cy.intercept()` to stub slow endpoints

#### Flaky tests

- Avoid fixed waits, use assertions instead
- Ensure proper element loading
- Check for async operations completion

#### Backend connection issues

- Verify backend is running with `npm run e2e:backend`
- Check that backend is using port 7266 (default E2E configuration)
- Ensure frontend proxy is configured correctly
