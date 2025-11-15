describe('Home Page', () => {
    beforeEach(() => {
        cy.visit('/');
    });

    it('should load the home page', () => {
        cy.url().should('include', '/');
        cy.get('body').should('be.visible');
    });

    it('should redirect unauthenticated users to login when accessing protected routes', () => {
        cy.visit('/profile');
        cy.url().should('include', '/identity/login');
    });

    it('should allow navigation to login page', () => {
        cy.get('[data-cy="login-link"]').click();
        cy.url().should('include', '/identity/login');
    });

    it('should allow navigation to register page', () => {
        cy.get('[data-cy="register-link"]').click();
        cy.url().should('include', '/identity/register');
    });

    it('should allow navigation to public landing page', () => {
        cy.login('test@example.com', 'testpassword');
        cy.visit('/');
        cy.get('[data-cy="landing-link"]').click();
        cy.url().should('eq', Cypress.config().baseUrl + '/');
    });

    it('should allow navigation to logout', () => {
        cy.login('test@example.com', 'testpassword');
        cy.visit('/');
        cy.get('[data-cy="logout-link"]').click();
        cy.shouldBeLoggedOut();
    });
});
