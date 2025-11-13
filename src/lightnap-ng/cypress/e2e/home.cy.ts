describe('Home Page', () => {
  beforeEach(() => {
    cy.visit('/')
  })

  it('should load the home page', () => {
    cy.url().should('include', '/')
    cy.get('body').should('be.visible')
  })

  it('should display the application title', () => {
    cy.get('[data-cy="logo-link"]').should('be.visible')
  })

  it('should have navigation elements', () => {
    cy.get('[data-cy="main-navigation"]').should('exist')
    cy.get('[data-cy="desktop-menu"]').should('exist')
  })
})
