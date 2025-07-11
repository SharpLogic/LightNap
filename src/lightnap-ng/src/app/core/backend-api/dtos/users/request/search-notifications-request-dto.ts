import { NotificationStatus, NotificationType, PagedRequestDto } from "@core";

/**
 * Interface representing a request to search for the user's notifications.
 * Extends the PaginationRequest interface to include pagination properties.
 */
export interface SearchNotificationsRequestDto extends PagedRequestDto {
  /**
   * The ID to filter notifications since. Returned notifications will be since this ID.
   * @type {number}
   */
  sinceId?: number;

  /**
   * The ID to filter notifications before. Returned notifications will be from prior to this ID.
   * @type {number}
   */
  priorToId?: number;

  /**
   * The status of the notifications to filter by.
   * @type {NotificationStatus}
   */
  status?: NotificationStatus;

  /**
   * The type of the notifications to filter by.
   * @type {NotificationType}
   */
  type?: NotificationType;
}
