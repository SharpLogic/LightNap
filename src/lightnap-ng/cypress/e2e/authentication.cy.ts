describe('Authentication', () => {
    beforeEach(() => {
        cy.visit('/');
    });

    it('should redirect to login when accessing protected route', () => {
        cy.visit('/profile');
        cy.url().should('include', '/identity/login');
    });

    it('should show login form', () => {
        cy.visit('/identity/login');
        cy.get('[data-cy="login-username"]').should('be.visible');
        cy.get('[data-cy="login-password"]').should('be.visible');
        cy.get('[data-cy="login-submit"]').should('be.visible');
    });

    it('should show error for invalid credentials', () => {
        cy.visit('/identity/login');
        cy.get('[data-cy="login-username"]').type('invalid@example.com');
        cy.get('[data-cy="login-password"]').type('wrongpassword');
        cy.get('[data-cy="login-submit"]').click();

        // Wait for the API response
        if (Cypress.env('useMocks')) {
            cy.wait('@loginRequest');
        }

        cy.get('.p-message-error, [data-cy="error-message"]').should(
            'be.visible',
        );
    });

    it('should login successfully with valid credentials', () => {
        cy.shouldBeLoggedOut();
        cy.login('test@example.com', 'testpassword');
        cy.shouldBeLoggedIn();
        cy.url().should('not.include', '/identity/login');
    });

    it('should logout successfully', () => {
        cy.login('test@example.com', 'testpassword');
        cy.logout();
        cy.shouldBeLoggedOut();
        cy.url().should('not.include', '/profile');
    });

    it('should maintain login state across page reloads', () => {
        cy.login('test@example.com', 'testpassword');
        cy.reload();
        cy.shouldBeLoggedIn();
    });
});
