using LightNap.Core.Extensions;
using LightNap.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LightNap.Core.Hubs
{
    /// <summary>
    /// SignalR hub that delivers real-time notifications to authenticated users via a strongly-typed
    /// <see cref="IRealTimeClient"/> contract.
    /// </summary>
    [Authorize]
    public class RealTimeHub(IUserContext userContext) : Hub<IRealTimeClient>
    {
        /// <summary>
        /// Called when a client connects to the hub. Adds the connection to the per-user group so the
        /// server can target push messages by user ID.
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            await this.AddToUserGroupAsync(Context.ConnectionId, userContext.GetUserId());
            await base.OnConnectedAsync();
        }
    }
}
