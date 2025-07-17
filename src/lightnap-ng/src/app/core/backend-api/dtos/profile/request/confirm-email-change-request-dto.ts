/**
 * Represents a request to confirm an email change.
 */
export interface ConfirmChangeEmailRequestDto {
    /**
     * The new email.
     */
    newEmail: string;

    /**
     * The verification code.
     */
    code: string;

}
