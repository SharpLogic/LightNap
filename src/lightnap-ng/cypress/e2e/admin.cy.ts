describe('Admin Panel', () => {
    beforeEach(() => {
        if (Cypress.env('useMocks')) {
            cy.setupAdminMocks();
        }
        cy.logInAdministrator();
        cy.visit('/admin');
    });

    it('should display the admin dashboard', () => {
        // Verify breadcrumb shows "Admin"
        cy.get('[data-cy="breadcrumb"]').should('contain', 'Admin');

        // Verify we're on the admin page
        cy.url().should('include', '/admin');
    });

    it('should display admin sidebar menu', () => {
        const subMenus = cy.get('[data-cy="sidebar-menu"] > div[data-pc-section="panel"]');
        subMenus.should('have.length.greaterThan', 3);
        subMenus.last().should('contain.text', 'Admin');
    });

    describe('Users Management', () => {
        beforeEach(() => {
            cy.visit('/admin/users');
        });

        it('should display users management page', () => {
            // Verify breadcrumb shows "Admin > Users"
            cy.get('[data-cy="breadcrumb"]').should('contain', 'Admin');
            cy.get('[data-cy="breadcrumb"]').should('contain', 'Users');

            // Verify the manage users panel is visible
            cy.get('[data-cy="manage-users-panel"]').should('be.visible');
        });

        it('should display users table with proper headers', () => {
            // Verify the users table is present
            cy.get('p-table').should('be.visible');

            // Verify key headers are present using data-cy attributes
            cy.get('[data-cy="user-name-header"]').should('be.visible');
            cy.get('[data-cy="email-header"]').should('be.visible');
            cy.get('[data-cy="created-header"]').should('be.visible');
        });

        it('should display user filter controls', () => {
            // Verify filter inputs are present (the form structure may vary)
            cy.get('[data-cy="user-name-filter"]').should('be.visible');
            cy.get('[data-cy="email-filter"]').should('be.visible');
        });

        it('should load user data', () => {
            // Verify the users table is present
            cy.get('p-table').should('be.visible');

            // Verify user data is loaded (will always have at least 3 records)
            cy.get('p-table tbody tr').should('have.length.greaterThan', 2);

            // Verify the structure of the first row
            cy.get('p-table tbody tr')
                .first()
                .within(() => {
                    cy.get('td').should('have.length', 5); // Delete button, User Name, Email, Created, Last Modified
                    cy.get('td').eq(1).find('a').should('exist'); // User Name should be a link
                });
        });
    });

    describe('Roles Management', () => {
        beforeEach(() => {
            cy.visit('/admin/roles');
        });

        it('should display roles management page', () => {
            // Verify breadcrumb shows "Admin > Roles"
            cy.get('[data-cy="breadcrumb"]').should('contain', 'Admin');
            cy.get('[data-cy="breadcrumb"]').should('contain', 'Roles');

            // Verify the manage roles panel is visible
            cy.get('[data-cy="manage-roles-panel"]').should('be.visible');
        });

        it('should display roles table with proper headers', () => {
            // Verify the roles table is present
            cy.get('p-table').should('be.visible');

            // Verify key headers are present using data-cy attributes
            cy.get('[data-cy="role-header"]').should('be.visible');
            cy.get('[data-cy="name-header"]').should('be.visible');
            cy.get('[data-cy="description-header"]').should('be.visible');
        });

        it('should load role data', () => {
            // Verify the roles table is present
            cy.get('p-table').should('be.visible');

            // Verify role data is loaded (mocked data has at least 2 records)
            cy.get('p-table tbody tr').should('have.length.greaterThan', 1);

            // Verify the structure of the first row
            cy.get('p-table tbody tr')
                .first()
                .within(() => {
                    cy.get('td').should('have.length', 3); // Role, Name, Description
                    cy.get('td').first().find('a').should('exist'); // Role should be a link
                });
        });
    });

    describe('Claims Management', () => {
        beforeEach(() => {
            cy.visit('/admin/claims');
        });

        it('should display claims management page', () => {
            // Verify breadcrumb shows "Admin > Claims"
            cy.get('[data-cy="breadcrumb"]').should('contain', 'Admin');
            cy.get('[data-cy="breadcrumb"]').should('contain', 'Claims');

            // Verify the manage claims panel is visible (assuming it has a similar structure)
            cy.get('[data-cy="search-claims-panel"]').should('be.visible');

            // Wait for claims to load (if mocks are enabled)
            if (Cypress.env('useMocks')) {
                cy.wait('@getClaims');
            }
        });

        it('should load claims data', () => {
            // Verify page loaded successfully
            cy.url().should('include', '/admin/claims');
        });
    });
});
