/**
 * Represents a request to send a magic link email.
 */
export interface SendMagicLinkEmailRequestDto {
    /**
     * The email address associated with the user's account.
     */
    email: string;
}
