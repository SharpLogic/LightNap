namespace LightNap.Configuration.Audit
{
    /// <summary>
    /// Retention settings for the administrative audit log.
    /// </summary>
    public sealed class AuditLogRetentionSettings
    {
        /// <summary>
        /// How many days to retain audit log entries. Entries with <c>CreatedAt</c> older than
        /// <c>UtcNow - RetentionDays</c> are removed by the purge maintenance task.
        /// </summary>
        public int RetentionDays { get; set; } = 365;
    }
}
