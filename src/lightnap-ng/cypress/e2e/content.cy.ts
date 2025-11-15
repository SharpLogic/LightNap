describe('Content Management', () => {
    beforeEach(() => {
        cy.loginContentEditor();
        cy.visit('/content');
    });

    it('should display the content submenu in breadcrumb', () => {
        // Verify breadcrumb shows "Content"
        cy.get('.p-breadcrumb').should('contain', 'Content');
    });

    it('should display the content overview list', () => {
        // Verify the manage contents panel is visible
        cy.get('.p-panel').contains('Manage Contents').should('be.visible');

        // Verify the content table is present
        cy.get('p-table').should('be.visible');

        // Check if there's content data
        cy.get('p-table tbody tr').should('have.length.greaterThan', 0);
    });

    it('should display filter controls', () => {
        // Verify filter form is present
        cy.get('form').should('be.visible');

        // Verify filter inputs are present
        cy.get('input[placeholder="Key contains"]').should('be.visible');
        cy.get('ln-content-status-picker').should('be.visible');
        cy.get('ln-content-type-picker').should('be.visible');
        cy.get('ln-content-read-access-picker').should('be.visible');
    });

    it('should display create content button', () => {
        // Verify create button is present in panel header
        cy.get('.p-panel .p-button').contains('Create').should('be.visible');
    });

    it('should load content data', () => {
        // Verify the content table is present
        cy.get('p-table').should('be.visible');

        // Check if there's content data
        cy.get('p-table tbody tr').should('have.length.greaterThan', 0);
    });
});
