// ***********************************************************
// This example support/e2e.ts is processed and
// loaded automatically before your test files.
//
// This is a great place to put global configuration and
// behavior that modifies Cypress.
//
// You can change the location of this file or turn off
// automatically serving support files with the
// 'supportFile' configuration option.
//
// You can read more here:
// https://on.cypress.io/configuration
// ***********************************************************

// Import commands.js using ES2015 syntax:
import './commands';
import { setupAuthMocks } from './mock-api';

// Handle uncaught exceptions to prevent test failures
Cypress.on('uncaught:exception', (err, runnable) => {
    // returning false here prevents Cypress from
    // failing the test
    console.log('Uncaught exception:', err.message);
    return false;
});

// Alternative: Log but still fail on exceptions
// Cypress.on('uncaught:exception', (err, runnable) => {
//   console.error('Uncaught exception:', err)
//   // Return false to prevent test failure, true to fail
//   return false
// })

// Alternatively you can use CommonJS syntax:
// require('./commands')

beforeEach(() => {
    if (Cypress.env('useMocks')) {
        setupAuthMocks();
    }
});
