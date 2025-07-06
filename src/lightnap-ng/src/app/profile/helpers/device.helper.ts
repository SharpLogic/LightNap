import { DeviceDto } from "@profile/models";

export class DeviceHelper {
    public static rehydrate(device: DeviceDto) {
        if (device?.lastSeen) {
            device.lastSeen = new Date(device.lastSeen);
        }
    }
}
