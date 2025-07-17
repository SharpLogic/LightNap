export interface NotificationItem {
  id: number;
  timestamp: Date;
  isUnread: boolean;
  title: string;
  description: string;
  icon?: string;
  imageUrl?: string;
  routerLink: Array<string>;
}
