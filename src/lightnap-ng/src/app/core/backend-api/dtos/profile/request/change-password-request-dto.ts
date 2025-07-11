/**
 * Represents a request to change a user's password.
 */
export interface ChangePasswordRequestDto {
    /**
     * The user's current password.
     */
    currentPassword: string;

    /**
     * The new password that the user wants to set.
     */
    newPassword: string;

    /**
     * Confirmation of the new password to ensure it was entered correctly.
     */
    confirmNewPassword: string;
}
