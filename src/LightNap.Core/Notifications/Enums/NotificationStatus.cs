namespace LightNap.Core.Notifications.Enums
{
    /// <summary>
    /// The user status of a notification.
    /// </summary>
    public enum NotificationStatus
    {
        /// <summary>
        /// The notification has not yet been read.
        /// </summary>
        Unread,

        /// <summary>
        /// The notification has been read but is still visible.
        /// </summary>
        Read,

        /// <summary>
        /// The notification is archived and should not be visible.
        /// </summary>
        Archived
    }
}
