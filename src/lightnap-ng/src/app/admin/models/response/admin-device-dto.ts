/**
 * Interface representing an admin device.
 */
export interface AdminDeviceDto {
    /**
     * The unique identifier for the device.
     * @type {string}
     */
    id: string;

    /**
     * The timestamp of the last time the device was seen.
     * @type {number}
     */
    lastSeen: number;

    /**
     * The IP address from the last time the device was seen.
     * @type {string}
     */
    ipAddress: string;

    /**
     * Device details, such as the browser agent, so it can be more easily identified by the user.
     * @type {string}
     */
    details: string;

    /**
     * The timestamp when the device expires.
     * @type {number}
     */
    expires: number;

    /**
     * The user associated with the device.
     * @type {string}
     */
    userId: string;
}
