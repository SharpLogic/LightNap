describe('Authentication with Mocks', () => {
  beforeEach(() => {
    // Mock the login API endpoint
    cy.intercept('POST', '**/api/auth/login', {
      statusCode: 200,
      body: {
        token: 'mock-jwt-token',
        user: {
          id: 1,
          email: 'test@example.com',
          firstName: 'Test',
          lastName: 'User'
        }
      }
    }).as('loginRequest')

    // Mock user profile endpoint
    cy.intercept('GET', '**/api/user/profile', {
      statusCode: 200,
      body: {
        id: 1,
        email: 'test@example.com',
        firstName: 'Test',
        lastName: 'User',
        roles: ['user']
      }
    }).as('getProfile')

    cy.visit('/identity/login')
  })

  it('should login successfully with mocked API', () => {
    cy.get('[data-cy="login-username"]').type('test@example.com')
    cy.get('[data-cy="login-password"]').type('password123')
    cy.get('[data-cy="login-submit"]').click()

    // Wait for the mocked login request
    cy.wait('@loginRequest')

    // Should redirect to home or dashboard
    cy.url().should('not.include', '/identity/login')
  })

  it('should show error for mocked invalid credentials', () => {
    // Override the mock to return an error
    cy.intercept('POST', '**/api/auth/login', {
      statusCode: 401,
      body: {
        message: 'Invalid credentials'
      }
    }).as('loginError')

    cy.get('[data-cy="login-username"]').type('wrong@example.com')
    cy.get('[data-cy="login-password"]').type('wrongpassword')
    cy.get('[data-cy="login-submit"]').click()

    cy.wait('@loginError')
    cy.get('.error, .alert-danger').should('be.visible')
  })
})

describe('Profile Management with Mocks', () => {
  beforeEach(() => {
    // Mock authentication check
    cy.intercept('GET', '**/api/auth/me', {
      statusCode: 200,
      body: {
        authenticated: true,
        user: {
          id: 1,
          email: 'test@example.com',
          firstName: 'Test',
          lastName: 'User'
        }
      }
    }).as('authCheck')

    // Mock profile data
    cy.intercept('GET', '**/api/user/profile', {
      statusCode: 200,
      body: {
        id: 1,
        email: 'test@example.com',
        firstName: 'Test',
        lastName: 'User',
        lastLogin: '2025-11-12T10:00:00Z',
        settings: {
          theme: 'light',
          notifications: true
        }
      }
    }).as('getProfile')

    // Set up authenticated session
    cy.window().then((win) => {
      win.localStorage.setItem('authToken', 'mock-jwt-token')
    })

    cy.visit('/profile')
  })

  it('should display mocked user profile', () => {
    cy.wait('@authCheck')
    cy.wait('@getProfile')

    cy.get('.profile-info, .user-details').should('be.visible')
    cy.contains('Test User').should('be.visible')
    cy.contains('test@example.com').should('be.visible')
  })

  it('should handle mocked API errors', () => {
    // Override profile endpoint to return error
    cy.intercept('GET', '**/api/user/profile', {
      statusCode: 500,
      body: {
        message: 'Internal server error'
      }
    }).as('profileError')

    cy.reload()
    cy.wait('@profileError')

    cy.get('.error, .alert-danger').should('be.visible')
  })
})
