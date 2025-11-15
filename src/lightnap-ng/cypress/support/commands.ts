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

// Custom command to login
Cypress.Commands.add('login', (email: string, password: string) => {
    //cy.session([email, password], () => {
    cy.visit('/identity/login');
    cy.get('[data-cy="login-username"]').should('be.visible');
    cy.get('[data-cy="login-username"]').type(email);
    cy.get('[data-cy="login-password"]').type(password);
    cy.get('[data-cy="login-submit"]').click();
    cy.url().should('not.include', '/identity/login');
    //});
});

Cypress.Commands.add('loginRegularUser', () => {
    cy.login('test@example.com', 'testpassword');
});

Cypress.Commands.add('loginAdministrator', () => {
    cy.login('admin@lightnap.sharplogic.com', 'adminpassword');
});

Cypress.Commands.add('loginContentManager', () => {
    cy.login(
        'contentmanager@lightnap.sharplogic.com',
        'contentmanagerpassword',
    );
});

// Custom command to logout
Cypress.Commands.add('logout', () => {
    cy.visit('/profile');
    cy.get('[data-cy="profile-logout"]').click();
    cy.url().should('not.include', '/profile');
});

// Custom command to check if user is logged in
Cypress.Commands.add('isLoggedIn', () => {
    if (Cypress.env('useMocks')) {
        return cy
            .getCookie('refreshToken', { domain: 'localhost' })
            .then((cookie) => !!cookie && cookie.value !== '');
    } else {
        return cy.getCookie('refreshToken').then((cookie) => !!cookie);
    }
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
            login(email: string, password: string): Chainable<void>;
            loginRegularUser(): Chainable<void>;
            loginAdministrator(): Chainable<void>;
            loginContentManager(): Chainable<void>;
            logout(): Chainable<void>;
            isLoggedIn(): Chainable<boolean>;
            shouldBeLoggedIn(): Chainable<void>;
            shouldBeLoggedOut(): Chainable<void>;
        }
    }
}

export {};
