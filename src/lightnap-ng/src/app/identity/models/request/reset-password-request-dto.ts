/**
 * Represents a request to reset a user's password.
 */
export interface ResetPasswordRequestDto {
    /**
     * The email address associated with the user's account.
     */
    email: string;
}
