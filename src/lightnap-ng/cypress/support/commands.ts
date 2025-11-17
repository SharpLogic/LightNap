// ***********************************************
// This example commands.ts shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************

/// <reference types="cypress" />

import { setupAdminMocks, setupContentMocks } from './mock-api';

// Custom command to login
function logIn(login: string, password: string) {
    if (Cypress.env('useMocks')) {
        // If mocking, we can use the Cypress session to cache login state
        cy.session([login, password], () => {
            logInInternal(login, password);
        });
    } else {
        // If running live we can't use the session because it will mess with the way
        // refresh token cookies are updated every time you request a new access token
        logInInternal(login, password);
    }
}

function logInInternal(login: string, password: string) {
    cy.visit('/identity/login');
    cy.get('[data-cy="login-username"]').should('be.visible');
    cy.get('[data-cy="login-username"]').type(login);
    cy.get('[data-cy="login-password"]').type(password);
    cy.get('[data-cy="login-submit"]').click();
    cy.url().should('not.include', '/identity/login');
}

Cypress.Commands.add('logInRegularUser', () => {
    logIn('E2eRegularUser', 'P@ssw0rd');
});

Cypress.Commands.add('logInAdministrator', () => {
    logIn('E2eAdmin', 'P@ssw0rd');
});

Cypress.Commands.add('logInContentEditor', () => {
    logIn('E2eContentEditor', 'P@ssw0rd');
});

Cypress.Commands.add('setupContentMocks', () => {
    setupContentMocks();
});

Cypress.Commands.add('setupAdminMocks', () => {
    setupAdminMocks();
});

// Custom command to logout
Cypress.Commands.add('logout', () => {
    cy.visit('/profile');
    cy.get('[data-cy="profile-logout"]').click();
    cy.url().should('not.include', '/profile');
});

// Custom command to check if user is logged in
Cypress.Commands.add('isLoggedIn', () => {
    return cy.getCookie('refreshToken').then((cookie) => !!cookie);
});

// Custom command to check if user is logged in
Cypress.Commands.add('shouldBeLoggedIn', () => {
    cy.isLoggedIn().should('be.true');
});

// Custom command to check if user is logged out
Cypress.Commands.add('shouldBeLoggedOut', () => {
    cy.isLoggedIn().should('be.false');
});

declare global {
    namespace Cypress {
        interface Chainable {
            logInRegularUser(): Chainable<void>;
            logInAdministrator(): Chainable<void>;
            logInContentEditor(): Chainable<void>;
            setupContentMocks(): Chainable<void>;
            setupAdminMocks(): Chainable<void>;
            logout(): Chainable<void>;
            isLoggedIn(): Chainable<boolean>;
            shouldBeLoggedIn(): Chainable<void>;
            shouldBeLoggedOut(): Chainable<void>;
        }
    }
}

export {};
