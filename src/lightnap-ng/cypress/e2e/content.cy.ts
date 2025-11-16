describe('Content Management', () => {
    beforeEach(() => {
        if (Cypress.env('useMocks')) {
            cy.setupContentMocks();
        }
        cy.logInContentEditor();
        cy.visit('/content');
    });

    it('should not be allowed into content section as a regular user', () => {
        cy.logInRegularUser();
        cy.visit('/content');
        cy.url().should('include', '/access-denied');
    });

    it('should display the content submenu in breadcrumb', () => {
        // Verify breadcrumb shows "Content"
        cy.get('[data-cy="breadcrumb"]').should('contain', 'Content');
    });

    it('should display content sidebar menu', () => {
        const subMenus = cy.get(
            '[data-cy="sidebar-menu"] > div[data-pc-section="panel"]',
        );
        subMenus.should('have.length.greaterThan', 2);
        subMenus.last().should('contain.text', 'Content');
    });

    it('should display the content overview list', () => {
        // Verify the manage contents panel is visible
        cy.get('[data-cy="manage-contents-panel"]').should('be.visible');

        // Verify the content table is present and has data
        cy.get('p-table').should('be.visible');
        cy.get('p-table tbody tr').should('have.length.greaterThan', 0);
    });

    it('should display filter controls', () => {
        // Verify filter form is present (simplified E2E check)
        cy.get('form, [formGroup]').should('be.visible');

        // Verify key filter input exists (main search functionality)
        cy.get('[data-cy="content-key-filter"]').should('be.visible');
    });

    it('should display create content button', () => {
        // Verify create button is present in panel header
        cy.get('[data-cy="create-content-button"]').should('be.visible');
    });

    it('should render edit link if viewed as editor', () => {
        cy.intercept('GET', '**/api/content/published/*/*', {
            statusCode: 200,
            body: {
                type: 'Success',
                result: {
                    visibility: 'Editor',
                    content: {
                        content: 'This is mocked published content.',
                        format: 'Html',
                    },
                },
            },
        }).as('getPublishedContent');
        cy.visit('/');
        cy.get('[data-cy="edit-content-link"]').should('be.visible');
    });
});
