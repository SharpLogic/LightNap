using LightNap.Core.Extensions;
using LightNap.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LightNap.Core.Hubs;

/// <summary>
/// A SignalR hub that manages real-time notifications for authenticated users.
/// </summary>
/// <remarks>Clients must be authenticated to connect to this hub. The hub enables sending targeted notifications
/// to individual users or groups based on their identity. Use this hub to facilitate real-time updates and alerts
/// within the application.</remarks>
/// <param name="userContext">The user context used to access information about the currently connected user.</param>
[Authorize]
public class RealTimeHub(IUserContext userContext) : Hub<IRealTimeClient>
{
    /// <summary>
    /// Called when a client connects to the hub.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        await this.AddToUserGroupAsync(Context.ConnectionId, userContext.GetUserId());
        await base.OnConnectedAsync();
    }
}