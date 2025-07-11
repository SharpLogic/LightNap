/**
 * Represents a request to change a user's email.
 */
export interface ChangeEmailRequestDto {
    /**
     * The new email that the user wants to set.
     */
    newEmail: string;
}
