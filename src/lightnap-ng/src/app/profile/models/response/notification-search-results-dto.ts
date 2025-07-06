import { PagedResponseDto } from "@core";
import { NotificationDto } from "./notification-dto";

/**
 * Represents a paged response of notifications.
 * @extends PagedResponseDto<NotificationDto>
 * @property {number} unreadCount - The number of unread notifications.
 */
export interface NotificationSearchResultsDto extends PagedResponseDto<NotificationDto> {
  unreadCount: number;
}
