namespace LightNap.Core.Data.Entities
{
    /// <summary>
    /// A single administrative audit record: who did what, when, and the before/after snapshots.
    /// </summary>
    public sealed class AdminAuditLog
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Stable actor identifier from <c>IUserContext.GetActorId()</c>: a user ID, a system
        /// sentinel, or (when anonymous visitor tracking is enabled) a visitor identifier.
        /// </summary>
        public required string ActorId { get; set; }

        /// <summary>
        /// Action name, typically dotted (e.g., <c>"user.deactivate"</c>, <c>"role.assign"</c>).
        /// </summary>
        public required string Action { get; set; }

        /// <summary>
        /// Optional. The entity type the action targets (e.g., <c>"User"</c>, <c>"Role"</c>).
        /// </summary>
        public string? TargetType { get; set; }

        /// <summary>
        /// Optional. The identifier of the target row.
        /// </summary>
        public string? TargetId { get; set; }

        /// <summary>
        /// JSON snapshot of the target before the action, when available.
        /// </summary>
        public string? BeforeJson { get; set; }

        /// <summary>
        /// JSON snapshot of the target after the action, or the action arguments when no
        /// explicit "after" object is supplied.
        /// </summary>
        public string? AfterJson { get; set; }

        /// <summary>
        /// UTC timestamp the entry was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
