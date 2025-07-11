import { NotificationDto } from "../dtos";

export class NotificationHelper {
    public static rehydrate(notification: NotificationDto) {
        if (notification?.timestamp) {
            notification.timestamp = new Date(notification.timestamp);
        }
    }
}
