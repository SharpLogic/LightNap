using System.Text.Json;
using LightNap.Core.Audit.Interfaces;
using LightNap.Core.Data;
using LightNap.Core.Data.Entities;
using LightNap.Core.Interfaces;

namespace LightNap.Core.Audit.Services
{
    /// <summary>
    /// Default <see cref="IAuditLogger"/> implementation: writes one <see cref="AdminAuditLog"/>
    /// row per call and resolves the actor from <see cref="IUserContext.GetActorId"/>.
    /// </summary>
    public sealed class AuditLogger(ApplicationDbContext db, IUserContext userContext) : IAuditLogger
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        /// <inheritdoc />
        public async Task WriteAsync(
            string action,
            string? targetType = null,
            string? targetId = null,
            object? before = null,
            object? after = null,
            CancellationToken cancellationToken = default)
        {
            db.AdminAuditLogs.Add(new AdminAuditLog
            {
                Id = Guid.NewGuid(),
                ActorId = userContext.GetUserId(),
                Action = action,
                TargetType = targetType,
                TargetId = targetId,
                BeforeJson = before is null ? null : JsonSerializer.Serialize(before, JsonOptions),
                AfterJson = after is null ? null : JsonSerializer.Serialize(after, JsonOptions),
                CreatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
