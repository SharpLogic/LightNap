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
  cy.session([email, password], () => {
    cy.visit('/identity/login')
    cy.get('[data-cy="login-username"]').type(email)
    cy.get('[data-cy="login-password"]').type(password)
    cy.get('[data-cy="login-submit"]').click()
    cy.url().should('not.include', '/identity/login')
  })
})

// Custom command to logout
Cypress.Commands.add('logout', () => {
  cy.get('[data-cy="profile-logout"]').click()
  cy.url().should('include', '/landing')
})

// Custom command to wait for loading to complete
Cypress.Commands.add('waitForLoading', () => {
  cy.get('body').should('not.have.class', 'loading')
})

// Custom command to check if user is logged in
Cypress.Commands.add('shouldBeLoggedIn', () => {
  cy.window().its('localStorage.token').should('exist')
})

// Custom command to check if user is logged out
Cypress.Commands.add('shouldBeLoggedOut', () => {
  cy.window().its('localStorage.token').should('not.exist')
})

declare global {
  namespace Cypress {
    interface Chainable {
      login(email: string, password: string): Chainable<void>
      logout(): Chainable<void>
      waitForLoading(): Chainable<void>
      shouldBeLoggedIn(): Chainable<void>
      shouldBeLoggedOut(): Chainable<void>
    }
  }
}

export {}
