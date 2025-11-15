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

Cypress.Commands.add('loginContentEditor', () => {
    cy.login(
        'contenteditor@lightnap.sharplogic.com',
        'contenteditorpassword',
    );
});

Cypress.Commands.add('setupContentMocks', () => {
    cy.intercept('POST', '**/api/content/search', {
        statusCode: 200,
        body: {
            type: 'Success',
            result: {
                data: [
                    {
                        key: 'sample-page',
                        type: 'Page',
                        status: 'Published',
                        readAccess: 'Public',
                        createdDate: '2024-01-01T00:00:00Z',
                        lastModifiedDate: '2024-01-01T00:00:00Z',
                    },
                    {
                        key: 'another-page',
                        type: 'Page',
                        status: 'Draft',
                        readAccess: 'Explicit',
                        createdDate: '2024-01-02T00:00:00Z',
                        lastModifiedDate: '2024-01-02T00:00:00Z',
                    },
                ],
                pageNumber: 1,
                pageSize: 10,
                totalCount: 2,
                totalPages: 1,
            },
        },
    }).as('searchContent');

    cy.intercept('POST', '**/api/content', {
        statusCode: 200,
        body: {
            type: 'Success',
            result: {
                key: 'new-content',
                type: 'Page',
                status: 'Draft',
                readAccess: 'Explicit',
                createdDate: '2024-01-03T00:00:00Z',
                lastModifiedDate: '2024-01-03T00:00:00Z',
            },
        },
    }).as('createContent');
});

Cypress.Commands.add('setupAdminMocks', () => {
    cy.intercept('POST', '**/api/users/search', {
        statusCode: 200,
        body: {
            type: 'Success',
            result: {
                data: [
                    {
                        id: 'user-1',
                        userName: 'admin',
                        email: 'admin@lightnap.sharplogic.com',
                        createdDate: '2024-01-01T00:00:00Z',
                        lastModifiedDate: '2024-01-01T00:00:00Z',
                        lockoutEnd: null,
                    },
                    {
                        id: 'user-2',
                        userName: 'contenteditor',
                        email: 'contenteditor@lightnap.sharplogic.com',
                        createdDate: '2024-01-02T00:00:00Z',
                        lastModifiedDate: '2024-01-02T00:00:00Z',
                        lockoutEnd: null,
                    },
                ],
                pageNumber: 1,
                pageSize: 10,
                totalCount: 2,
                totalPages: 1,
            },
        },
    }).as('getUsers');

    cy.intercept('GET', '**/api/users/roles', {
        statusCode: 200,
        body: {
            type: 'Success',
            result: [
                {
                    name: 'Administrator',
                    displayName: 'Administrator',
                    description: 'Full administrative access to the system',
                },
                {
                    name: 'ContentEditor',
                    displayName: 'Content Editor',
                    description: 'Can manage content in the system',
                },
            ],
        },
    }).as('getRoles');

    cy.intercept('POST', '**/api/users/claims/search', {
        statusCode: 200,
        body: {
            type: 'Success',
            result: {
                data: [
                    {
                        type: 'Content:Reader',
                        value: 'sample-page',
                    },
                    {
                        type: 'Content:Editor',
                        value: 'another-page',
                    },
                ],
                pageNumber: 1,
                pageSize: 10,
                totalCount: 2,
                totalPages: 1,
            },
        },
    }).as('getClaims');
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
            login(email: string, password: string): Chainable<void>;
            loginRegularUser(): Chainable<void>;
            loginAdministrator(): Chainable<void>;
            loginContentEditor(): Chainable<void>;
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
