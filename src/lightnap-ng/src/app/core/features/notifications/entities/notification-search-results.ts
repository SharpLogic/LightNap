import { NotificationSearchResultsDto } from "@core/backend-api";
import { NotificationItem as NotificationItem } from "./notification-item";

export interface NotificationSearchResults extends NotificationSearchResultsDto {
  notifications: Array<NotificationItem>;
}
