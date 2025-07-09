/**
 * Represents a user profile.
 */
export interface ProfileDto {
    /**
     * The unique identifier of the profile.
     */
    id: string;

    /**
     * The email address associated with the profile.
     */
    email: string;

    /**
     * The username of the profile.
     */
    userName: string;
}
