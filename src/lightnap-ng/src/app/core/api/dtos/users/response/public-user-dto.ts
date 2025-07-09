/**
 * Interface representing a user with limited details for a public context.
 */
export interface PublicUserDto {
    /**
     * The unique identifier for the user.
     * @type {string}
     */
    id: string;

    /**
     * The username of the user.
     * @type {string}
     */
    userName: string;

    /**
     * The timestamp when the user was created.
     * @type {number}
     */
    createdDate: number;
}
