import { DeviceDto } from "../dtos";

export class DeviceHelper {
    public static rehydrate(device: DeviceDto) {
        if (device?.lastSeen) {
            device.lastSeen = new Date(device.lastSeen);
        }
    }
}
