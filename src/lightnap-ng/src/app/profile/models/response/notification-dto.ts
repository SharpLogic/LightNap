import { NotificationStatus } from "../notifications/notification-status";
import { NotificationType } from "../notifications/notification-type";

/**
 * Represents an application notification.
 *
 * @interface Notification
 * @property {number} id - The unique identifier for the notification.
 * @property {Date} timestamp - The date and time when the notification was created.
 * @property {NotificationStatus} status - The current status of the notification.
 * @property {NotificationType} type - The type/category of the notification.
 * @property {any} data - Additional data associated with the notification.
 */
export interface NotificationDto {
  id: number;
  timestamp: Date;
  status: NotificationStatus;
  type: NotificationType;
  data: any;
}
