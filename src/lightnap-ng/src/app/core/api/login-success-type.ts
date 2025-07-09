/**
 * Represents the possible outcomes of a successful login attempt.
 *
 * @typedef {("AccessToken" | "EmailVerificationRequired" | "TwoFactorRequired")} LoginSuccessType
 *
 * @property {"AccessToken"} AccessToken - Indicates that the login was successful and an access token was provided.
 * @property {"EmailVerificationRequired"} EmailVerificationRequired - Indicates that the login was successful but email verification is required.
 * @property {"TwoFactorRequired"} TwoFactorRequired - Indicates that the login was successful but two-factor authentication is required.
 */
export type LoginSuccessType = "AccessToken" | "EmailVerificationRequired" | "TwoFactorRequired";
