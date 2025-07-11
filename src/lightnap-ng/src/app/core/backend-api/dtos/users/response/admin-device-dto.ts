import { DeviceDto } from "./device-dto";

/**
 * Interface representing the full details of a device for an administrative context
 */
export interface AdminDeviceDto extends DeviceDto {
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
