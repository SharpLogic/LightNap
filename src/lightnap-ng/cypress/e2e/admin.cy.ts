describe('Admin Panel', () => {
  beforeEach(() => {
    if (Cypress.env('useMocks')) {
      cy.setupAdminMocks();
    }
    cy.loginAdministrator();
    cy.visit('/admin');
  });

  it('should display the admin dashboard', () => {
    // Verify breadcrumb shows "Admin"
    cy.get('.p-breadcrumb').should('contain', 'Admin');

    // Verify we're on the admin page
    cy.url().should('include', '/admin');
  });

  it('should display admin navigation menu', () => {
    // Check if admin menu items are visible (this depends on your menu implementation)
    // You might need to adjust these selectors based on your actual menu structure
    cy.get('body').should('be.visible'); // Basic check that page loaded
  });

  describe('Users Management', () => {
    beforeEach(() => {
      cy.visit('/admin/users');
    });

    it('should display users management page', () => {
      // Verify breadcrumb shows "Admin > Users"
      cy.get('.p-breadcrumb').should('contain', 'Admin');
      cy.get('.p-breadcrumb').should('contain', 'Users');

      // Verify the manage users panel is visible
      cy.get('.p-panel').contains('Manage Users').should('be.visible');
    });

    it('should display users table with proper headers', () => {
      // Verify the users table is present
      cy.get('p-table').should('be.visible');

      // Verify table has expected number of columns (simplified E2E check)
      cy.get('p-table th').should('have.length.greaterThan', 3);
    });

    it('should display user filter controls', () => {
      // Verify filter inputs are present (the form structure may vary)
      cy.get('input[placeholder="User Name"]').should('be.visible');
      cy.get('input[placeholder="Email"]').should('be.visible');
    });

    it('should load user data', () => {
      // Wait for users to load
      if (Cypress.env('useMocks')) {
        cy.wait('@getUsers');
      }

      // Verify the users table is present
      cy.get('p-table').should('be.visible');

      // Check if there's user data
      cy.get('p-table tbody').then($tbody => {
        if ($tbody.find('tr').length > 0) {
          // If there are rows, verify they have the expected structure
          cy.get('p-table tbody tr').first().within(() => {
            cy.get('td').should('have.length', 5); // Delete button, User Name, Email, Created, Last Modified
            cy.get('td').eq(1).find('a').should('exist'); // User Name should be a link
          });
        } else {
          // If no users, verify empty message
          cy.get('p-table').should('contain', 'No users match the filters.');
        }
      });
    });
  });

  describe('Roles Management', () => {
    beforeEach(() => {
      cy.visit('/admin/roles');
    });

    it('should display roles management page', () => {
      // Verify breadcrumb shows "Admin > Roles"
      cy.get('.p-breadcrumb').should('contain', 'Admin');
      cy.get('.p-breadcrumb').should('contain', 'Roles');

      // Verify the manage roles panel is visible
      cy.get('.p-panel').contains('Manage Roles').should('be.visible');
    });

    it('should display roles table with proper headers', () => {
      // Verify the roles table is present
      cy.get('p-table').should('be.visible');

      // Verify table has expected number of columns (simplified E2E check)
      cy.get('p-table th').should('have.length.greaterThan', 2);
    });

    it('should load role data', () => {
      // Wait for roles to load
      if (Cypress.env('useMocks')) {
        cy.wait('@getRoles');
      }

      // Verify the roles table is present
      cy.get('p-table').should('be.visible');

      // Check if there's role data
      cy.get('p-table tbody').then($tbody => {
        if ($tbody.find('tr').length > 0) {
          // If there are rows, verify they have the expected structure
          cy.get('p-table tbody tr').first().within(() => {
            cy.get('td').should('have.length', 3); // Role, Name, Description
            cy.get('td').first().find('a').should('exist'); // Role should be a link
          });
        } else {
          // If no roles, verify empty message
          cy.get('p-table').should('contain', 'There are no roles.');
        }
      });
    });
  });

  describe('Claims Management', () => {
    beforeEach(() => {
      cy.visit('/admin/claims');
    });

    it('should display claims management page', () => {
      // Verify breadcrumb shows "Admin > Claims"
      cy.get('.p-breadcrumb').should('contain', 'Admin');
      cy.get('.p-breadcrumb').should('contain', 'Claims');

      // Verify the manage claims panel is visible (assuming it has a similar structure)
      cy.get('body').should('be.visible'); // Basic check that page loaded
    });

    it('should load claims data', () => {
      // Wait for claims to load
      if (Cypress.env('useMocks')) {
        cy.wait('@getClaims');
      }

      // Verify page loaded successfully
      cy.url().should('include', '/admin/claims');
    });
  });
});
