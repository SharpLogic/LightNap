describe('Content Management', () => {
    beforeEach(() => {
        if (Cypress.env('useMocks')) {
            cy.setupContentMocks();
        }
        cy.loginContentEditor();
        cy.visit('/content');
    });

    it('should display the content submenu in breadcrumb', () => {
        // Verify breadcrumb shows "Content"
        cy.get('.p-breadcrumb').should('contain', 'Content');
    });

    it('should display the content overview list', () => {
        // Verify the manage contents panel is visible
        cy.get('.p-panel').should('be.visible');

        // Verify the content table is present and has data
        cy.get('p-table').should('be.visible');
        cy.get('p-table tbody tr').should('have.length.greaterThan', 0);
    });

    it('should display filter controls', () => {
        // Verify filter form is present (simplified E2E check)
        cy.get('form, [formGroup]').should('be.visible');

        // Verify key filter input exists (main search functionality)
        cy.get('input[pinputtext]').should('be.visible');
    });

    it('should display create content button', () => {
        // Verify create button is present in panel header
        cy.get('.p-panel .p-button').should('be.visible');
    });
});
