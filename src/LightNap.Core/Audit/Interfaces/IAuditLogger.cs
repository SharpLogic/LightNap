namespace LightNap.Core.Audit.Interfaces
{
    /// <summary>
    /// Records administrative actions to the audit log.
    /// </summary>
    public interface IAuditLogger
    {
        /// <summary>
        /// Persists an audit log entry. The actor is resolved from <c>IUserContext.GetActorId()</c>.
        /// </summary>
        /// <param name="action">Action name (e.g., <c>"user.deactivate"</c>).</param>
        /// <param name="targetType">Optional target entity type.</param>
        /// <param name="targetId">Optional target row identifier.</param>
        /// <param name="before">Optional before-snapshot. Serialized to JSON when not null.</param>
        /// <param name="after">Optional after-snapshot. Serialized to JSON when not null.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task WriteAsync(
            string action,
            string? targetType = null,
            string? targetId = null,
            object? before = null,
            object? after = null,
            CancellationToken cancellationToken = default);
    }
}
