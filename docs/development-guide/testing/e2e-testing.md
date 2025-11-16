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
  cy.get('[data-cy="login-username"]').type('user@example.com');
  cy.get('[data-cy="login-password"]').type('password');
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

Before running E2E tests, make sure you have the frontend development server running.

```bash
npm run start
```

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

{: .note }
You can also run `npm run e2e` or `npm run e2e:open` to run against a live backend. Use the [seeding support](../../development-guide/data-persistence/backend-seeding) to automatically configure backends for different environments.

### CI Mode

```bash
npm run e2e:ci
```

Runs tests with mocks, recording, and parallel execution for CI environments.

## Test Organization

### File Structure

```
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

## Best Practices

### Test Data Management
- Use fixtures for static data
- Seed test data programmatically when needed
- Clean up data between tests

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

**Tests fail intermittently**
- Increase timeouts in `cypress.config.ts`
- Use `cy.wait()` for async operations
- Check for race conditions

**Element not found**
- Verify `data-cy` attributes exist
- Check if element is rendered after navigation
- Use `cy.get()` with timeout options

**API calls not intercepted**
- Ensure intercept is set before the action
- Check URL patterns match exactly
- Use `cy.intercept()` with wildcards

**Slow test execution**
- Enable API mocking
- Reduce viewport size if not needed
- Use `cy.intercept()` to stub slow endpoints

**Flaky tests**
- Avoid fixed waits, use assertions instead
- Ensure proper element loading
- Check for async operations completion