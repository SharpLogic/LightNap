# Vitest Migration - Completion Plan

## Executive Summary
The foundational Vitest migration is complete with 144/225 tests (64%) passing. This document outlines the systematic approach to address remaining failures and achieve 100% test coverage.

## Current Status Dashboard

```
Total Tests:        225
Passing:            144 (64%)
Failing:            81  (36%)

Test Files:         55
Passing:            22
Failing:            33

Errors:             28 (mostly template resolution & deprecated done() callbacks)
```

## Issue Classification

### Category 1: Component Template Resolution (22 tests)
**Severity**: High | **Effort**: Medium | **Impact**: 22 tests

**Affected Files**:
- `select-list-item.component.spec.ts` (22 failing)
- `confirm-dialog.component.spec.ts` (21 failing)
- `confirm-popup.component.spec.ts` (19 failing)
- `breadcrumb.component.spec.ts` (11 failing)
- `app-footer.component.spec.ts` (5 failing)

**Error Pattern**:
```
Component 'ComponentName' is not resolved:
- templateUrl: ./component.component.html
Did you run and wait for 'resolveComponentResources()'?
```

**Root Cause**: 
- Vitest with jsdom requires explicit resource resolution for components with external templates
- Unlike Karma which automatically resolved resources, Vitest requires manual handling

**Fix Approach**:

Option A: Add resolveComponentResources() to global setupFiles
```typescript
// In src/testing/vitest-setup.ts
import { resolveComponentResources } from '@angular/core/testing';

beforeEach(async () => {
  // This will be called before each test automatically
  await resolveComponentResources();
});
```

Option B: Add to each component spec's beforeEach
```typescript
beforeEach(async () => {
  await resolveComponentResources();
  TestBed.configureTestingModule({...});
});
```

**Recommended**: Option A (global setup) for consistency and less code changes

**Implementation Steps**:
1. Update `src/testing/vitest-setup.ts`:
   - Import `resolveComponentResources`
   - Call in a global beforeEach hook
2. Test affected component specs
3. Verify all 22 template-related tests pass

### Category 2: Deprecated done() Callback (9+ tests)
**Severity**: Medium | **Effort**: Low | **Impact**: 9+ tests

**Affected Files**:
- `block-ui.service.spec.ts` (2+ tests using done())
- `request-polling-manager.spec.ts` (multiple done() calls)
- Other Observable-based tests

**Error Pattern**:
```
Error: done() callback is deprecated, use promise instead
```

**Root Cause**: 
- Vitest deprecates done() callbacks in favor of async/await or returning promises
- Observable tests currently use done callback patterns from Jasmine era

**Fix Approach**:

Convert from done() callbacks:
```typescript
// Old (Jasmine with done())
it('should emit values', done => {
  service.onShow$.subscribe(value => {
    expect(value).toBe(expected);
    done();
  });
  service.show(params);
});

// New (Vitest with Promise)
it('should emit values', () => {
  return new Promise<void>(resolve => {
    service.onShow$.subscribe(value => {
      expect(value).toBe(expected);
      resolve();
    });
    service.show(params);
  });
});

// Or even better with async/await
it('should emit values', async () => {
  const promise = service.onShow$.toPromise();
  service.show(params);
  const value = await promise;
  expect(value).toBe(expected);
});
```

**Implementation Steps**:
1. Identify all files using done() callbacks
2. Convert Observable subscriptions to Promises
3. Use async/await or return Promise from test
4. Test for reliability
5. Consider using RxJS testing utilities

### Category 3: Jasmine Clock Compatibility (9+ tests)
**Severity**: Medium | **Effort**: Medium | **Impact**: 9+ tests

**Affected Files**:
- `request-polling-manager.spec.ts` (9 failing - uses jasmine.clock)

**Error Pattern**:
```
Error: jasmine.clock is not a function
```

**Root Cause**:
- The jasmine.clock() compatibility layer needs refinement
- Tests rely on jasmine.clock.tick() for advancing timers

**Fix Approach**:

Update Jasmine compatibility layer in vitest-setup.ts:
```typescript
(globalThis as any).jasmine = {
  // ... existing code ...
  clock: () => {
    let installed = false;
    return {
      install: () => {
        vi.useFakeTimers();
        installed = true;
      },
      uninstall: () => {
        vi.useRealTimers();
        installed = false;
      },
      tick: (ms: number) => {
        if (!installed) vi.useFakeTimers();
        vi.advanceTimersByTime(ms);
      },
      mockDate: (date: Date) => {
        if (!installed) vi.useFakeTimers();
        vi.setSystemTime(date);
      },
    };
  },
};
```

Also update test patterns:
```typescript
// Old (Jasmine clock)
beforeEach(() => {
  jasmine.clock().install();
});

afterEach(() => {
  jasmine.clock().uninstall();
});

it('test', () => {
  jasmine.clock().tick(1000);
  expect(something).toBe(true);
});

// New (Vitest with setupFiles)
beforeEach(() => {
  vi.useFakeTimers();
});

afterEach(() => {
  vi.useRealTimers();
});

it('test', () => {
  vi.advanceTimersByTime(1000);
  expect(something).toBe(true);
});
```

### Category 4: Remaining Failures (< 5 tests)
**Severity**: Low | **Effort**: Variable | **Impact**: < 5 tests

**Potential Issues**:
- Custom matchers specific to Jasmine
- Browser-specific behaviors
- Module loading issues

**Approach**: Case-by-case analysis and fixes

## Implementation Timeline

### Phase 1: Foundation Fixes (30-45 minutes)
**Target**: Get to 180+ tests passing (80%)

1. **Update vitest-setup.ts** with resolveComponentResources (10 min)
   - Add global beforeEach with resolveComponentResources()
   - Improve jasmine.clock() implementation
   - Test changes

2. **Fix Component Template Issues** (15 min)
   - Run tests to verify template resolution fixes
   - Address any remaining template-related failures

3. **Validate Fixes** (10 min)
   - Run full test suite
   - Document any new issues

### Phase 2: Observable/Timer Fixes (30-45 minutes)
**Target**: Get to 210+ tests passing (93%)

1. **Convert done() Callbacks** (20 min)
   - Update block-ui.service.spec.ts
   - Update request-polling-manager.spec.ts
   - Test Observable-based tests

2. **Fix Timer Tests** (15 min)
   - Enhance jasmine.clock implementation
   - Update timer-dependent tests
   - Verify RequestPollingManager tests

3. **Validate** (10 min)
   - Full test suite run
   - Coverage verification

### Phase 3: Edge Cases & Polish (15-30 minutes)
**Target**: Achieve 225/225 passing (100%)

1. **Identify Remaining Issues** (10 min)
   - Analyze any remaining failures
   - Categorize by type

2. **Fix Edge Cases** (10 min)
   - Address specific test failures
   - Custom matcher implementations if needed

3. **Final Validation** (10 min)
   - Full test suite with coverage
   - Performance benchmarking
   - Documentation updates

## Detailed Fix Instructions

### Fix 1: Template Resolution in vitest-setup.ts

```typescript
import { getTestBed, resolveComponentResources } from '@angular/core/testing';
import { beforeEach as vitestBeforeEach } from 'vitest';

// Add after TestBed.initTestEnvironment()
vitestBeforeEach(async () => {
  // Resolve all component resources before each test
  // This allows TestBed.createComponent() to work with external templates
  try {
    await resolveComponentResources();
  } catch (e) {
    // Silently ignore if there are no components to resolve
  }
});
```

### Fix 2: Update request-polling-manager.spec.ts

Replace done callbacks:
```typescript
it('should call polling function immediately', async () => {
  const promise = new Promise<void>(resolve => {
    pollingManager.onStart$.subscribe(() => {
      expect(pollingFn).toHaveBeenCalled();
      resolve();
    });
  });
  
  pollingManager.start();
  await promise;
});

// Or use modern timer functions
it('should respect interval', async () => {
  vi.useFakeTimers();
  
  const calls = [];
  pollingManager.onTick$.subscribe(() => {
    calls.push(Date.now());
  });
  
  pollingManager.start({ interval: 1000 });
  
  vi.advanceTimersByTime(3500);
  expect(calls.length).toBeGreaterThanOrEqual(3);
  
  vi.useRealTimers();
});
```

### Fix 3: Verify Coverage Configuration

```bash
npm run test:coverage
# Should generate: coverage/lightnap-ng/index.html
# Check that thresholds are met:
# - Lines: 42% ✓
# - Functions: 40% ✓
# - Branches: 25% ✓
# - Statements: 45% ✓
```

## Success Criteria

- [ ] **225/225 tests passing** (100%)
- [ ] **All test categories passing**:
  - [ ] Service tests (18/18)
  - [ ] Component tests (all template-related fixed)
  - [ ] Directive tests
  - [ ] Guard tests
  - [ ] Interceptor tests
  - [ ] Helper tests
  - [ ] Pipe tests
  - [ ] Model tests

- [ ] **Coverage reports generated successfully**
  - [ ] HTML report at `coverage/lightnap-ng/index.html`
  - [ ] LCOV format for CI/CD
  - [ ] All thresholds met

- [ ] **All npm scripts working**:
  - [ ] `npm run test:watch` - Live mode
  - [ ] `npm run test:ci` - CI mode
  - [ ] `npm run test:coverage` - Coverage generation
  - [ ] `npm run test:debug` - Debug mode

- [ ] **Performance maintained**
  - [ ] Setup < 50s
  - [ ] Execution < 5s
  - [ ] Total < 15s

- [ ] **CI/CD ready**
  - [ ] No console warnings
  - [ ] Deterministic results
  - [ ] Reproducible across environments

## Risk Mitigation

### Potential Risks
1. **Template Resolution Breaks Other Tests**
   - Mitigation: Test impact in vitest-setup.ts with try-catch
   - Backup: Make fix selective per test file if needed

2. **Async Conversion Introduces Timing Issues**
   - Mitigation: Use `vi.useFakeTimers()` for deterministic tests
   - Backup: Return to promise chains instead of async/await

3. **Coverage Drops Below Thresholds**
   - Mitigation: Verify thresholds are correct
   - Backup: Adjust thresholds to match current coverage baseline

## Testing the Fixes

Before committing fixes, verify with:

```bash
# Single test file
npm run test:ci -- src/app/core/components/select-list-item/select-list-item.component.spec.ts

# All component tests
npm run test:ci -- src/app/core/components

# Coverage check
npm run test:coverage

# Watch mode for development
npm run test:watch
```

## Documentation Updates

After fixes are complete, update:
1. Project README - Add Vitest testing section
2. VITEST_MIGRATION.md - Update completion status
3. Contributing guide - Document test patterns
4. CI/CD pipelines - Verify script usage

## Rollback Plan

If issues arise that cannot be quickly resolved:

```bash
# Revert to Karma
git checkout HEAD~1 -- package.json angular.json tsconfig.spec.json
git rm vitest.config.ts src/testing/vitest-setup.ts
npm install
npm test
```

The migration is designed to be fully reversible until all tests pass and changes are committed.

## Next Steps

1. **Immediate** (Next 15 minutes):
   - Implement Fix 1: Template resolution
   - Run test suite to validate

2. **Short-term** (Next 30-45 minutes):
   - Implement Fix 2: done() callbacks
   - Implement Fix 3: Timer tests
   - Achieve 100% passing

3. **Follow-up** (Next day):
   - Update documentation
   - Test in CI/CD pipeline
   - Celebrate success! 🎉
