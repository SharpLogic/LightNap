---
title: Unit Testing with Karma/Jasmine
layout: home
parent: Testing
nav_order: 10
---

# {{ page.title }}

LightNap uses **Karma** as the test runner and **Jasmine** as the testing framework for unit testing Angular components, services, and utilities. This guide covers how to write, configure, and run unit tests effectively.

## Test Configuration

Unit tests are configured in `karma.conf.js` with the following key settings:

- **Frameworks**: Jasmine and Angular DevKit for seamless Angular testing
- **Browsers**: Chrome (with headless options for CI)
- **Coverage**: Istanbul reporter with configurable thresholds
- **Reporters**: Progress and HTML reporters for development

### Coverage Thresholds

The configuration enforces minimum coverage levels:

```javascript
check: {
  global: {
    statements: 45,
    branches: 25,
    functions: 40,
    lines: 42,
  },
}
```

Tests will fail if coverage drops below these thresholds.

## Writing Unit Tests

### Basic Component Test Structure

```typescript
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { MyComponent } from './my.component';

describe('MyComponent', () => {
  let component: MyComponent;
  let fixture: ComponentFixture<MyComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [MyComponent],
      providers: [
        provideZonelessChangeDetection(),
        provideNoopAnimations(),
      ],
    });

    fixture = TestBed.createComponent(MyComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
```

### Key Testing Patterns

#### Testing Components with Inputs/Outputs

```typescript
it('should emit value on button click', () => {
  spyOn(component.valueChange, 'emit');

  const button = fixture.debugElement.query(By.css('button'));
  button.triggerEventHandler('click', null);

  expect(component.valueChange.emit).toHaveBeenCalledWith('expected value');
});
```

#### Testing Services

```typescript
describe('MyService', () => {
  let service: MyService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [MyService]
    });

    service = TestBed.inject(MyService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  it('should fetch data', () => {
    service.getData().subscribe(data => {
      expect(data).toEqual(expectedData);
    });

    const req = httpMock.expectOne('/api/data');
    expect(req.request.method).toBe('GET');
    req.flush(expectedData);
  });
});
```

#### Using Test Helpers

LightNap provides testing utilities in `src/testing/`:

```typescript
import { TestModule } from '@testing/test.module';
import { UserDtoBuilder } from '@testing/builders';

describe('UserService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [TestModule],
      providers: [UserService]
    });
  });

  it('should handle user data', () => {
    const testUser = new UserDtoBuilder().build();
    // Test implementation
  });
});
```

## Running Tests Locally

### Development Mode (with watch)

```bash
npm run test
# or
ng test
```

This runs tests in watch mode, re-running on file changes.

### Debug Mode

```bash
npm run test:debug
```

Opens Chrome for debugging with source maps.

### Headless Mode

```bash
npm run test:headless
```

Runs tests in headless Chrome without UI.

## Coverage Reports

### Generating Coverage

```bash
npm run test:coverage
```

### Viewing Reports

```bash
npm run test:coverage:open
```

Opens the HTML coverage report in your browser showing:
- Statement, branch, function, and line coverage
- Uncovered lines highlighted
- Detailed breakdown by file

### Coverage Files

Reports are generated in `coverage/lightnap-ng/`:
- `index.html`: Interactive HTML report
- `lcov-report/`: LCOV format for CI tools
- `coverage-summary.json`: JSON summary for automation

## Best Practices

### Test Organization
- Place test files next to implementation: `component.spec.ts`
- Use descriptive test names: `it('should display error for invalid input')`
- Group related tests in `describe` blocks

### Mocking
- Use Angular's `HttpTestingController` for HTTP calls
- Mock services with `jasmine.createSpyObj`
- Use LightNap's test builders for complex objects

### Async Testing
- Use `done` callback for observables
- Prefer `fakeAsync` and `tick` for simpler async tests
- Use `waitForAsync` for component initialization

### Performance
- Keep tests fast and focused
- Avoid unnecessary setup in `beforeEach`
- Use `TestBed.inject()` instead of manual instantiation

## Troubleshooting

### Common Issues

**Tests fail with "Can't bind to 'property' since it isn't a known property"**
- Ensure component is properly imported in TestBed configuration
- Check for missing module imports

**HTTP tests hang**
- Ensure all HTTP requests are flushed with `httpMock.verify()`

**Coverage not updating**
- Clear `coverage/` directory and re-run tests
- Check karma configuration for correct paths

**Slow test execution**
- Use `TestBed.overrideComponent()` to avoid full compilation
- Mock heavy dependencies