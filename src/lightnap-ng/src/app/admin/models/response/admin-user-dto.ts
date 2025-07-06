/**
 * Interface representing a user with full details for an administrative context.
 */
export interface AdminUserDto {
    /**
     * The unique identifier for the user.
     * @type {string}
     */
    id: string;

    /**
     * The email address of the user.
     * @type {string}
     */
    email: string;

    /**
     * The username of the user.
     * @type {string}
     */
    userName: string;

    /**
     * The timestamp when the user was last modified.
     * @type {number}
     */
    lastModifiedDate: number;

    /**
     * The timestamp when the user was created.
     * @type {number}
     */
    createdDate: number;

    /**
     * The timestamp when the user lockout ends. If the user is not locked out, this value is undefined.
     * @type {number}
     */
    lockoutEnd: number;
}
