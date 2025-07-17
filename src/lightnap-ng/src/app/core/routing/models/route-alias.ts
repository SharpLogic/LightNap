/**
 * Known route aliases for the app. Add more here and then associate them with routes set up for a given
 * page using the AppRoute.data.alias property.
 */
export type RouteAlias =
    // Public
    | 'landing'
    | 'about'
    | 'terms-and-conditions'
    | 'privacy-policy'
    | 'access-denied'
    | 'error'
    | 'not-found'

    // User
    | 'user-home'

    // Admin
    | 'admin-home'
    | 'admin-users'
    | 'admin-user'
    | 'admin-roles'
    | 'admin-role'
    | 'admin-claims'
    | 'admin-claim'

    // Profile
    | 'profile'
    | 'devices'
    | 'notifications'
    | 'change-email'
    | 'change-email-requested'

    // Identity
    | 'login'
    | 'magic-link-sent'
    | 'login'
    | 'reset-password'
    | 'reset-instructions-sent'
    | 'change-password'
    | 'verify-code'
    | 'register'
    | 'email-verification-required'
    | 'request-verification-email'
    | 'confirm-email';
