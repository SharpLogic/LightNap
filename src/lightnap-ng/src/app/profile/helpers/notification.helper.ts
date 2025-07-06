import { NotificationDto } from "@profile/models/response/notification-dto";

export class NotificationHelper {
    public static rehydrate(notification: NotificationDto) {
        if (notification?.timestamp) {
            notification.timestamp = new Date(notification.timestamp);
        }
    }
}
