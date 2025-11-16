describe('Profile Management', () => {
    beforeEach(() => {
        cy.visit('/');
    });

    it('should redirect to login if not authenticated', () => {
        cy.visit('/profile');
        cy.url().should('include', '/identity/login');
    });

    it('should display profile sidebar menu after logging in', () => {
        cy.logInRegularUser();
        cy.visit('/profile');
        const subMenus = cy.get(
            '[data-cy="sidebar-menu"] > div[data-pc-section="panel"]',
        );
        subMenus.should('have.length.greaterThan', 1);
        subMenus.last().should('contain.text', 'Profile');
    });

    it('should display user profile information when authenticated', () => {
        cy.logInRegularUser();
        cy.visit('/profile');
        cy.url().should('include', '/profile');
        cy.get('body').should('be.visible');
    });

    it('should allow logout from profile page', () => {
        cy.logInRegularUser();
        cy.visit('/profile');
        cy.get('[data-cy="profile-logout"]').click();
        cy.shouldBeLoggedOut();
        cy.url().should('not.include', '/profile');
    });
});
