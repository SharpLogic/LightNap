# Testing Guide for LightNap Angular Application

This guide provides comprehensive information on testing practices, utilities, and patterns for the LightNap Angular front-end application.

## Table of Contents

- [Testing Setup](#testing-setup)
- [Running Tests](#running-tests)
- [Testing Utilities](#testing-utilities)
- [Testing Patterns](#testing-patterns)
- [Best Practices](#best-practices)
- [Examples](#examples)

---

## Testing Setup

### Framework

LightNap uses **Jasmine** as the testing framework and **Karma** as the test runner. Tests are written in TypeScript and co-located with the source files using the `.spec.ts` extension.

### Test Structure

```
src/
  app/
    core/
      services/
        identity.service.ts
        identity.service.spec.ts  ← Test file
  testing/                        ← Testing utilities
    mocks/                        ← Mock services
    builders/                     ← Test data builders
    matchers/                     ← Custom matchers
    test.module.ts                ← Shared testing module
    test-helpers.ts               ← Helper functions
```

### Coverage Thresholds

Current minimum coverage thresholds:
- **Statements**: 50%
- **Branches**: 40%
- **Functions**: 50%
- **Lines**: 50%

These will increase as test coverage improves.

---

## Running Tests

### Available Commands

```bash
# Run tests in watch mode (default)
npm test

# Run tests once (CI mode)
npm run test:ci

# Run tests with coverage report
npm run test:coverage

# Run tests with coverage and open report
npm run test:coverage:open

# Run tests in headless mode
npm run test:headless

# Debug tests in Chrome
npm run test:debug
```

### Coverage Reports

Coverage reports are generated in `coverage/lightnap-ng/`:
- `index.html` - Interactive HTML report
- `lcov.info` - LCOV format for CI tools
- `coverage-summary.json` - JSON summary

---

## Testing Utilities

### Importing Testing Utilities

```typescript
import {
  // Mock services
  MockIdentityService,
  MockToastService,
  MockRouteAliasService,

  // Builders
  IdentityDtoBuilder,
  UserDtoBuilder,
  PagedResponseBuilder,

  // Matchers
  addCustomMatchers,

  // Helpers
  firstValueFrom,
  createSpyObj,
  queryByCss,
  clickElement,

  // Module
  SharedTestingModule,
} from 'src/testing';
```

### Mock Services

Pre-built mock services for common dependencies:

#### MockIdentityService

```typescript
const mockIdentity = new MockIdentityService();

// Configure logged-in state
mockIdentity.setLoggedIn('mock-token', ['Administrator']);

// Configure logged-out state
mockIdentity.setLoggedOut();

// Check states
expect(mockIdentity.hasUserRole('Administrator')).toBe(true);
```

#### MockToastService

```typescript
const mockToast = new MockToastService();

// In your test
mockToast.success('Success!');

// Verify
expect(mockToast.messages.length).toBe(1);
expect(mockToast.hasMessage('success')).toBe(true);
```

#### MockRouteAliasService

```typescript
const mockRouter = new MockRouteAliasService();

// In your test
mockRouter.navigate('home');

// Verify
expect(mockRouter.lastNavigatedAlias).toBe('home');
expect(mockRouter.navigationCount).toBe(1);
```

### Test Data Builders

Easily create test data with builder patterns:

#### IdentityDtoBuilder

```typescript
// Create a valid token
const token = IdentityDtoBuilder.createTestToken();

// Create an expired token
const expiredToken = IdentityDtoBuilder.createExpiredToken();

// Create a token with custom claims
const tokenWithClaims = IdentityDtoBuilder.createTokenWithClaims({
  role: 'Administrator',
  email: 'admin@example.com',
});

// Create login request
const loginRequest = IdentityDtoBuilder.createLoginRequest({
  userName: 'testuser',
  password: 'password123',
});
```

#### UserDtoBuilder

```typescript
// Create a single user
const user = UserDtoBuilder.createUser();

// Create multiple users
const users = UserDtoBuilder.createUsers(5);

// Create user with specific roles
const admin = UserDtoBuilder.createUserWithRoles(['Administrator']);

// Create admin user
const adminUser = UserDtoBuilder.createAdminUser();

// Reset counter for test isolation
UserDtoBuilder.resetCounter();
```

#### PagedResponseBuilder

```typescript
const users = UserDtoBuilder.createUsers(10);

// Create paged response
const pagedResponse = PagedResponseBuilder.create(users);

// Create empty response
const emptyResponse = PagedResponseBuilder.createEmpty();

// Create multi-page response
const multiPage = PagedResponseBuilder.createMultiPage(
  users.slice(0, 10),
  1,      // page
  10,     // pageSize
  50      // totalCount
);
```

### Custom Matchers

Domain-specific matchers for clearer assertions:

```typescript
beforeEach(() => {
  addCustomMatchers();
});

it('should have valid token', () => {
  const token = IdentityDtoBuilder.createTestToken();
  expect(token).toBeValidToken();
});

it('should have expired token', () => {
  const token = IdentityDtoBuilder.createExpiredToken();
  expect(token).toBeExpiredToken();
});

it('should have role', () => {
  const user = UserDtoBuilder.createUserWithRoles(['Admin']);
  expect(user).toHaveRole('Admin');
});

it('should have claim', () => {
  const user = UserDtoBuilder.createUserWithClaims([
    { type: 'permission', value: 'read' }
  ]);
  expect(user).toHaveClaim('permission', 'read');
});
```

### Test Helpers

Common helper functions for testing:

```typescript
// Wait for observables
const value = await firstValueFrom(observable$);

// Collect multiple emissions
const values = await collectValues(observable$, 3);

// Query DOM elements
const button = queryByCss(fixture, 'button.submit');
const items = queryAllByCss(fixture, '.list-item');

// Interact with elements
clickElement(fixture, 'button.submit');
setInputValue(fixture, 'input[name="email"]', 'test@example.com');

// Get text content
const text = getTextContent(fixture, '.message');

// Create mock objects
const httpError = createHttpError(404, 'Not Found');
const routeSnapshot = createMockActivatedRouteSnapshot({ params: { id: '123' } });
```

---

## Testing Patterns

### Service Testing

```typescript
import { TestBed } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { MyService } from './my.service';
import { MockIdentityService, createSpyObj } from 'src/testing';

describe('MyService', () => {
  let service: MyService;
  let mockIdentity: MockIdentityService;

  beforeEach(() => {
    mockIdentity = new MockIdentityService();

    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        MyService,
        { provide: IdentityService, useValue: mockIdentity },
      ],
    });

    service = TestBed.inject(MyService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should handle logged in user', () => {
    mockIdentity.setLoggedIn('token', ['User']);
    expect(service.canDoSomething()).toBe(true);
  });
});
```

### Component Testing

```typescript
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MyComponent } from './my.component';
import { SharedTestingModule, MockToastService, clickElement } from 'src/testing';

describe('MyComponent', () => {
  let component: MyComponent;
  let fixture: ComponentFixture<MyComponent>;
  let mockToast: MockToastService;

  beforeEach(async () => {
    mockToast = new MockToastService();

    await TestBed.configureTestingModule({
      imports: [SharedTestingModule, MyComponent],
      providers: [
        { provide: ToastService, useValue: mockToast },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(MyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should show success message on submit', () => {
    clickElement(fixture, 'button[type="submit"]');

    expect(mockToast.hasMessage('success')).toBe(true);
  });
});
```

### Guard Testing

```typescript
import { TestBed } from '@angular/core/testing';
import { MyGuard } from './my.guard';
import { MockIdentityService, createMockActivatedRouteSnapshot } from 'src/testing';

describe('MyGuard', () => {
  let guard: MyGuard;
  let mockIdentity: MockIdentityService;

  beforeEach(() => {
    mockIdentity = new MockIdentityService();

    TestBed.configureTestingModule({
      providers: [
        MyGuard,
        { provide: IdentityService, useValue: mockIdentity },
      ],
    });

    guard = TestBed.inject(MyGuard);
  });

  it('should allow access for authenticated users', () => {
    mockIdentity.setLoggedIn('token');

    const route = createMockActivatedRouteSnapshot();
    const result = guard.canActivate(route, null as any);

    expect(result).toBe(true);
  });

  it('should deny access for unauthenticated users', () => {
    mockIdentity.setLoggedOut();

    const route = createMockActivatedRouteSnapshot();
    const result = guard.canActivate(route, null as any);

    expect(result).toBe(false);
  });
});
```

### Interceptor Testing

```typescript
import { TestBed } from '@angular/core/testing';
import { HttpRequest, HttpResponse } from '@angular/common/http';
import { of } from 'rxjs';
import { myInterceptor } from './my.interceptor';

describe('myInterceptor', () => {
  let next: jasmine.Spy;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    next = jasmine.createSpy().and.returnValue(of(new HttpResponse({ status: 200 })));
  });

  it('should add authorization header', (done) => {
    const request = new HttpRequest('GET', '/api/test');

    TestBed.runInInjectionContext(() => {
      myInterceptor(request, next).subscribe(() => {
        const modifiedRequest = next.calls.mostRecent().args[0];
        expect(modifiedRequest.headers.has('Authorization')).toBe(true);
        done();
      });
    });
  });
});
```

### Pipe Testing

```typescript
import { MyPipe } from './my.pipe';

describe('MyPipe', () => {
  let pipe: MyPipe;

  beforeEach(() => {
    pipe = new MyPipe();
  });

  it('should transform value', () => {
    expect(pipe.transform('input')).toBe('expected-output');
  });

  it('should handle null', () => {
    expect(pipe.transform(null)).toBe('');
  });
});
```

### Directive Testing

```typescript
import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MyDirective } from './my.directive';
import { SharedTestingModule } from 'src/testing';

@Component({
  template: '<div myDirective>Test</div>',
  standalone: true,
  imports: [MyDirective],
})
class TestComponent {}

describe('MyDirective', () => {
  let fixture: ComponentFixture<TestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SharedTestingModule, TestComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TestComponent);
    fixture.detectChanges();
  });

  it('should apply directive behavior', () => {
    const div = fixture.nativeElement.querySelector('div');
    expect(div.classList.contains('directive-applied')).toBe(true);
  });
});
```

---

## Best Practices

### General Principles

1. **Test Behavior, Not Implementation**
   - Focus on what the code does, not how it does it
   - Avoid testing private methods directly

2. **Arrange-Act-Assert Pattern**
   ```typescript
   it('should do something', () => {
     // Arrange - set up test data
     const input = 'test';

     // Act - execute the code
     const result = service.doSomething(input);

     // Assert - verify the result
     expect(result).toBe('expected');
   });
   ```

3. **One Assertion Per Test (When Possible)**
   - Makes failures easier to diagnose
   - Use multiple assertions only for related checks

4. **Use Descriptive Test Names**
   ```typescript
   // Good
   it('should return empty array when no users exist', () => {});

   // Bad
   it('test users', () => {});
   ```

5. **Test Edge Cases**
   - Empty arrays/strings
   - Null/undefined values
   - Boundary conditions
   - Error scenarios

### Angular-Specific

1. **Use `provideZonelessChangeDetection()` for Services**
   ```typescript
   TestBed.configureTestingModule({
     providers: [provideZonelessChangeDetection(), MyService],
   });
   ```

2. **Import SharedTestingModule for Components**
   ```typescript
   await TestBed.configureTestingModule({
     imports: [SharedTestingModule, MyComponent],
   }).compileComponents();
   ```

3. **Clean Up Subscriptions**
   ```typescript
   it('should handle observable', (done) => {
     service.getData().subscribe(data => {
       expect(data).toBeDefined();
       done(); // Complete the async test
     });
   });
   ```

4. **Use `fixture.detectChanges()` for Component Updates**
   ```typescript
   component.property = 'new value';
   fixture.detectChanges(); // Trigger change detection
   ```

### Test Organization

1. **Group Related Tests with `describe`**
   ```typescript
   describe('MyService', () => {
     describe('authentication', () => {
       it('should login', () => {});
       it('should logout', () => {});
     });

     describe('authorization', () => {
       it('should check permissions', () => {});
     });
   });
   ```

2. **Use `beforeEach` for Common Setup**
   ```typescript
   beforeEach(() => {
     // Runs before each test
   });
   ```

3. **Use `afterEach` for Cleanup**
   ```typescript
   afterEach(() => {
     // Runs after each test
   });
   ```

### Mocking

1. **Use Provided Mock Services**
   ```typescript
   const mockIdentity = new MockIdentityService();
   ```

2. **Create Spies for Method Verification**
   ```typescript
   const spy = jasmine.createSpy('methodName');
   spy.and.returnValue('result');
   ```

3. **Use `jasmine.createSpyObj` for Multiple Methods**
   ```typescript
   const mockService = jasmine.createSpyObj('MyService', [
     'method1',
     'method2',
   ]);
   ```

---

## Examples

### Example 1: Testing a Service with Dependencies

```typescript
import { TestBed } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { UserService } from './user.service';
import { MockIdentityService, UserDtoBuilder } from 'src/testing';
import { of } from 'rxjs';

describe('UserService', () => {
  let service: UserService;
  let mockIdentity: MockIdentityService;
  let httpSpy: jasmine.SpyObj<any>;

  beforeEach(() => {
    mockIdentity = new MockIdentityService();
    httpSpy = jasmine.createSpyObj('HttpClient', ['get', 'post', 'delete']);

    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        UserService,
        { provide: IdentityService, useValue: mockIdentity },
        { provide: HttpClient, useValue: httpSpy },
      ],
    });

    service = TestBed.inject(UserService);
  });

  it('should get users when authenticated', (done) => {
    // Arrange
    mockIdentity.setLoggedIn('token', ['Administrator']);
    const mockUsers = UserDtoBuilder.createUsers(3);
    httpSpy.get.and.returnValue(of(mockUsers));

    // Act
    service.getUsers().subscribe(users => {
      // Assert
      expect(users.length).toBe(3);
      expect(httpSpy.get).toHaveBeenCalledWith(jasmine.stringContaining('users'));
      done();
    });
  });
});
```

### Example 2: Testing a Component with Form

```typescript
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LoginComponent } from './login.component';
import {
  SharedTestingModule,
  MockIdentityService,
  MockToastService,
  IdentityDtoBuilder,
  setInputValue,
  clickElement,
} from 'src/testing';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let mockIdentity: MockIdentityService;
  let mockToast: MockToastService;

  beforeEach(async () => {
    mockIdentity = new MockIdentityService();
    mockToast = new MockToastService();

    await TestBed.configureTestingModule({
      imports: [SharedTestingModule, LoginComponent],
      providers: [
        { provide: IdentityService, useValue: mockIdentity },
        { provide: ToastService, useValue: mockToast },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should submit login form', () => {
    // Arrange
    spyOn(mockIdentity, 'logIn').and.returnValue(
      of(IdentityDtoBuilder.createLoginSuccessResult())
    );

    // Act
    setInputValue(fixture, 'input[name="login"]', 'testuser');
    setInputValue(fixture, 'input[name="password"]', 'password123');
    clickElement(fixture, 'button[type="submit"]');

    // Assert
    expect(mockIdentity.logIn).toHaveBeenCalled();
  });

  it('should show error on invalid credentials', () => {
    // Arrange
    spyOn(mockIdentity, 'logIn').and.returnValue(
      throwError(() => new Error('Invalid credentials'))
    );

    // Act
    setInputValue(fixture, 'input[name="login"]', 'wronguser');
    setInputValue(fixture, 'input[name="password"]', 'wrongpass');
    clickElement(fixture, 'button[type="submit"]');

    // Assert
    expect(mockToast.hasMessage('error')).toBe(true);
  });
});
```

### Example 3: Testing with Custom Matchers

```typescript
import { IdentityService } from './identity.service';
import {
  IdentityDtoBuilder,
  addCustomMatchers,
  MockIdentityDataService,
} from 'src/testing';

describe('IdentityService', () => {
  let service: IdentityService;
  let mockData: MockIdentityDataService;

  beforeEach(() => {
    addCustomMatchers(); // Enable custom matchers

    mockData = new MockIdentityDataService();
    TestBed.configureTestingModule({
      providers: [
        IdentityService,
        { provide: IdentityDataService, useValue: mockData },
      ],
    });

    service = TestBed.inject(IdentityService);
  });

  it('should have valid token after login', (done) => {
    const token = IdentityDtoBuilder.createTestToken();
    spyOn(mockData, 'logIn').and.returnValue(
      of({ accessToken: token, type: 'AccessToken' })
    );

    service.logIn(IdentityDtoBuilder.createLoginRequest()).subscribe(() => {
      const bearerToken = service.getBearerToken();
      expect(bearerToken).toContain(token);
      expect(token).toBeValidToken(); // Custom matcher
      done();
    });
  });

  it('should detect expired token', () => {
    const expiredToken = IdentityDtoBuilder.createExpiredToken();
    expect(expiredToken).toBeExpiredToken(); // Custom matcher
  });
});
```

---

## Additional Resources

- [Jasmine Documentation](https://jasmine.github.io/)
- [Angular Testing Guide](https://angular.io/guide/testing)
- [Karma Configuration](https://karma-runner.github.io/latest/config/configuration-file.html)

---

## Contributing to Tests

When adding new features:

1. Write tests first (TDD) or alongside implementation
2. Ensure all new code has test coverage
3. Run tests locally before committing: `npm run test:ci`
4. Check coverage report: `npm run test:coverage:open`
5. Add new testing utilities to `src/testing/` if they're reusable

---

**Questions or Issues?** Please reach out to the development team or create an issue in the repository.
