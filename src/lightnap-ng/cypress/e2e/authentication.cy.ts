describe('Authentication', () => {
  beforeEach(() => {
    cy.visit('/')
  })

  it('should redirect to login when accessing protected route', () => {
    cy.visit('/profile')
    cy.url().should('include', '/identity/login')
  })

  it('should show login form', () => {
    cy.visit('/identity/login')
    cy.get('[data-cy="login-username"]').should('be.visible')
    cy.get('[data-cy="login-password"]').should('be.visible')
    cy.get('[data-cy="login-submit"]').should('be.visible')
  })

  it('should show error for invalid credentials', () => {
    cy.visit('/identity/login')
    cy.get('[data-cy="login-username"]').type('invalid@example.com')
    cy.get('[data-cy="login-password"]').type('wrongpassword')
    cy.get('.p-message-error').should('not.exist')
    cy.get('[data-cy="login-submit"]').click()
    cy.get('.p-message-error').should('be.visible')
  })

  // Note: This test requires valid test credentials
  // Uncomment and update with actual test user credentials
  /*
  it('should login successfully with valid credentials', () => {
    cy.login('test@example.com', 'testpassword')
    cy.shouldBeLoggedIn()
    cy.url().should('not.include', '/identity/login')
  })

  it('should logout successfully', () => {
    cy.login('test@example.com', 'testpassword')
    cy.logout()
    cy.shouldBeLoggedOut()
    cy.url().should('include', '/identity/login')
  })
  */
})
