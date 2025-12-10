# Testing with Vitest

This project has been migrated from Karma/Jasmine to Vitest. Use the npm scripts below instead of `ng test`.

## Available Commands

### Development (Watch Mode)
```bash
npm run test:watch
```
Runs tests in watch mode with live reload.

### CI/Headless (Single Run)
```bash
npm run test:ci
```
Runs all tests once and exits. Use for CI/CD pipelines.

### Coverage Report
```bash
npm run test:coverage
```
Generates HTML coverage report at `coverage/lightnap-ng/index.html`

### Debug Mode
```bash
npm run test:debug
```
Runs tests with Node debugger enabled. Attach debugger in Chrome DevTools.

### Run Specific File
```bash
npm run test:watch -- src/app/core/services/identity.service.spec.ts
```

### Run Tests Matching Pattern
```bash
npm run test:watch -- --grep "IdentityService"
```

## Why Not `ng test`?

The Angular CLI's `ng test` command was bound to Karma, which has been removed. Vitest provides better performance and a more modern testing experience. All test functionality is available through the npm scripts above.

## Vitest Configuration

- **Config File**: `vitest.config.ts`
- **Setup File**: `src/testing/vitest-setup.ts`
- **Coverage Threshold**: 45% statements, 40% functions, 25% branches, 42% lines

## Documentation

See [VITEST_MIGRATION.md](./VITEST_MIGRATION.md) for detailed migration information.
