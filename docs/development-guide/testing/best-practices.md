---
title: Testing Best Practices
layout: home
parent: Testing
nav_order: 30
---

# {{ page.title }}

This guide outlines best practices for testing in LightNap, covering strategies, mocking, coverage, and integration with development workflows.

## Testing Strategy

### Testing Pyramid

LightNap follows a balanced testing pyramid:

1. **Unit Tests** (Base): Test individual functions and components
2. **Integration Tests**: Test component interactions and services
3. **E2E Tests** (Top): Test complete user workflows

### When to Write Tests

- **New Features**: Write tests alongside implementation
- **Bug Fixes**: Add regression tests
- **Refactoring**: Ensure tests pass after changes
- **Critical Paths**: Test authentication, data operations, user flows

## Unit Testing Best Practices

### Test Structure

```typescript
describe('ComponentName', () => {
  describe('when initialized', () => {
    it('should display default state', () => {
      // Test implementation
    });
  });

  describe('when user interacts', () => {
    it('should update state accordingly', () => {
      // Test implementation
    });
  });
});
```

### Mocking Strategy

#### Service Mocking

```typescript
const mockService = jasmine.createSpyObj('MyService', ['method1', 'method2']);
mockService.method1.and.returnValue(of(testData));

TestBed.configureTestingModule({
  providers: [
    { provide: MyService, useValue: mockService }
  ]
});
```

#### HTTP Mocking

```typescript
import { HttpTestingController } from '@angular/common/http/testing';

it('should fetch data', () => {
  service.getData().subscribe();

  const req = httpMock.expectOne('/api/data');
  expect(req.request.method).toBe('GET');
  req.flush(mockData);

  httpMock.verify();
});
```

### Using Test Builders

LightNap provides builders for consistent test data:

```typescript
import { UserDtoBuilder } from '@testing/builders';

const testUser = new UserDtoBuilder()
  .withName('John Doe')
  .withEmail('john@example.com')
  .build();
```

## E2E Testing Best Practices

### Page Object Pattern

```typescript
// cypress/support/page-objects/login-page.ts
export class LoginPage {
  visit() {
    cy.visit('/identity/login');
    return this;
  }

  fillCredentials(username: string, password: string) {
    cy.get('[data-cy="login-username"]').type(username);
    cy.get('[data-cy="login-password"]').type(password);
    return this;
  }

  submit() {
    cy.get('[data-cy="login-submit"]').click();
    return this;
  }
}

// Usage
const loginPage = new LoginPage();
loginPage.visit().fillCredentials('user', 'pass').submit();
```

### Custom Commands

```typescript
// cypress/support/commands.ts
Cypress.Commands.add('login', (username: string, password: string) => {
  cy.session([username, password], () => {
    cy.visit('/identity/login');
    cy.get('[data-cy="login-username"]').type(username);
    cy.get('[data-cy="login-password"]').type(password);
    cy.get('[data-cy="login-submit"]').click();
    cy.url().should('not.include', '/identity/login');
  });
});

// Usage
cy.login('admin', 'password');
```

## Coverage Goals

### Target Coverage Levels

- **Statements**: 45% (current threshold)
- **Branches**: 25%
- **Functions**: 40%
- **Lines**: 42%

### Coverage Analysis

Focus coverage on:
- **Business Logic**: Service methods, complex calculations
- **User Interactions**: Component event handlers
- **Error Handling**: Exception paths, validation
- **Edge Cases**: Boundary conditions, error states

Accept lower coverage for:
- **UI Boilerplate**: Simple getters/setters, template logic
- **Generated Code**: Auto-generated files
- **Trivial Code**: Simple property bindings

## CI/CD Integration

### Running Tests in Pipeline

```yaml
# .github/workflows/ci.yml
- name: Run Unit Tests
  run: npm run test:ci

- name: Run E2E Tests
  run: npm run e2e:ci

- name: Upload Coverage
  uses: codecov/codecov-action@v3
  with:
    file: ./coverage/lcov.info
```

### Parallel Execution

```bash
# Run E2E tests in parallel
npm run e2e:ci
# Configured with --record --parallel in package.json
```

### Test Reporting

- **Coverage Reports**: Upload to Codecov or similar
- **Test Results**: Store JUnit XML for CI dashboards
- **Screenshots**: Capture on E2E failures

## Debugging Tests

### Unit Test Debugging

```typescript
it('should debug component behavior', () => {
  spyOn(console, 'log'); // Capture console output

  component.debugMethod();

  expect(console.log).toHaveBeenCalledWith('Debug info');
});
```

### E2E Debugging

```typescript
it('should debug user flow', () => {
  cy.visit('/page');

  // Pause execution
  cy.pause();

  // Debug element
  cy.get('[data-cy="element"]').debug();

  // Log information
  cy.get('[data-cy="element"]').then($el => {
    cy.log('Element text:', $el.text());
  });
});
```

## Test Maintenance

### Keeping Tests Green

- **Regular Execution**: Run tests frequently during development
- **CI Monitoring**: Fix broken tests immediately
- **Refactor Tests**: Update tests when code changes
- **Remove Flaky Tests**: Replace unreliable tests with better implementations

### Test Code Quality

- **DRY Principle**: Extract common test setup
- **Descriptive Names**: Clear test and describe blocks
- **Documentation**: Comment complex test scenarios
- **Performance**: Keep tests fast and focused

## Common Pitfalls

### Avoid These Patterns

**Testing Implementation Details**
```typescript
// Bad: Testing private methods
expect(component['privateMethod']()).toBe(true);

// Good: Test public behavior
component.publicAction();
expect(component.publicProperty).toBe(expectedValue);
```

**Over-Mocking**
```typescript
// Bad: Mocking everything
const mockEverything = jasmine.createSpyObj('Service', ['method1', 'method2', ...]);

// Good: Mock only what's needed
const mockService = jasmine.createSpyObj('Service', ['neededMethod']);
```

**Brittle Selectors**
```typescript
// Bad: CSS-dependent
cy.get('.btn-primary').click();

// Good: Semantic
cy.get('[data-cy="submit-button"]').click();
```

**Race Conditions**
```typescript
// Bad: Fixed waits
cy.wait(3000);

// Good: Assertions
cy.get('[data-cy="result"]').should('be.visible');
```

## Tools and Resources

### Recommended Tools

- **Wallaby.js**: Real-time test execution in IDE
- **Cypress Dashboard**: Cloud-based test management
- **TestCafe**: Alternative E2E framework
- **Jest**: Alternative unit testing framework

### Learning Resources

- [Angular Testing Guide](https://angular.io/guide/testing)
- [Cypress Documentation](https://docs.cypress.io)
- [Jasmine Documentation](https://jasmine.github.io)
- [Testing Library](https://testing-library.com)