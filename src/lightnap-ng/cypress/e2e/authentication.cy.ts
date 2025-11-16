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

        cy.get('.p-message-error, [data-cy="error-message"]').should(
            'be.visible',
        );
    });

    it('should login successfully with valid credentials', () => {
        cy.shouldBeLoggedOut();
        cy.logInRegularUser();
        cy.shouldBeLoggedIn();
        cy.url().should('not.include', '/identity/login');
    });

    it('should logout successfully', () => {
        cy.logInRegularUser();
        cy.logout();
        cy.shouldBeLoggedOut();
        cy.url().should('not.include', '/profile');
    });

    it('should maintain login state across page reloads', () => {
        cy.logInRegularUser();
        cy.reload();
        cy.shouldBeLoggedIn();
    });

    it('should not enable registration button unless all form requirements are met', () => {
        cy.visit('/identity/register');

        // Initially disabled
        cy.get('[data-cy="register-submit"]')
            .find('button')
            .should('be.disabled');

        // Fill userName
        cy.get('[data-cy="register-username"]').type('testuser');
        cy.get('[data-cy="register-submit"]')
            .find('button')
            .should('be.disabled');

        // Fill email
        cy.get('[data-cy="register-email"]').type('test@example.com');
        cy.get('[data-cy="register-submit"]')
            .find('button')
            .should('be.disabled');

        // Fill password
        cy.get('[data-cy="register-password"]')
            .find('input')
            .type('password123');
        cy.get('[data-cy="register-submit"]')
            .find('button')
            .should('be.disabled');

        // Fill confirmPassword
        cy.get('[data-cy="register-confirm-password"]')
            .find('input')
            .type('password123');
        cy.get('[data-cy="register-submit"]')
            .find('button')
            .should('be.disabled');

        // Check agreedToTerms
        cy.get('[data-cy="register-agreed-to-terms"]').find('input').check();
        cy.get('[data-cy="register-submit"]')
            .find('button')
            .should('be.enabled');
    });
});
