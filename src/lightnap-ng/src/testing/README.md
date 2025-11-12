# Testing Utilities

This directory contains reusable testing utilities for the LightNap Angular application.

## Structure

- **`mocks/`** - Mock implementations of services for testing
- **`builders/`** - Test data builders and factories
- **`matchers/`** - Custom Jasmine matchers
- **`test.module.ts`** - Shared testing module with common imports
- **`test-helpers.ts`** - Utility functions for testing

## Quick Start

```typescript
import {
  MockIdentityService,
  IdentityDtoBuilder,
  SharedTestingModule,
  addCustomMatchers,
} from 'src/testing';

describe('MyComponent', () => {
  beforeEach(() => {
    addCustomMatchers();

    TestBed.configureTestingModule({
      imports: [SharedTestingModule, MyComponent],
      providers: [
        { provide: IdentityService, useClass: MockIdentityService },
      ],
    });
  });

  it('should work', () => {
    const token = IdentityDtoBuilder.createTestToken();
    expect(token).toBeValidToken();
  });
});
```

## Documentation

See **[TESTING.md](../TESTING.md)** in the root directory for comprehensive testing guide.

## Available Utilities

### Mock Services

- `MockIdentityService` - Mock authentication service
- `MockIdentityDataService` - Mock HTTP identity service
- `MockToastService` - Mock notification service
- `MockRouteAliasService` - Mock routing service
- `MockInitializationService` - Mock app initialization
- `MockTimerService` - Mock timer service
- `MockProfileService` - Mock profile service

### Builders

- `IdentityDtoBuilder` - Create identity-related test data
- `UserDtoBuilder` - Create user-related test data
- `PagedResponseBuilder` - Create paged response test data

### Custom Matchers

- `toBeValidToken()` - Check if JWT token is valid
- `toBeExpiredToken()` - Check if JWT token is expired
- `toHaveRole(role)` - Check if user has role
- `toHaveClaim(type, value)` - Check if user has claim
- `toHaveBeenCalledWithPartial(obj)` - Partial object matching

### Helper Functions

- `firstValueFrom()` - Get first value from observable
- `collectValues()` - Collect multiple observable emissions
- `createSpyObj()` - Create typed spy objects
- `queryByCss()` - Query DOM elements
- `clickElement()` - Click and detect changes
- `setInputValue()` - Set input value and trigger events
- Plus many more...

## Adding New Utilities

When adding reusable testing utilities:

1. Add to the appropriate subdirectory
2. Export from the subdirectory's `index.ts`
3. Update this README
4. Add examples to TESTING.md
