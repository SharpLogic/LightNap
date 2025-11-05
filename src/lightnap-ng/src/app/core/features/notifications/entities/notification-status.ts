export type NotificationStatus = "Read" | "Unread" | "Archived";

export const NotificationStatus = {
    Read: "Read",
    Unread: "Unread",
    Archived: "Archived",
} as const;
