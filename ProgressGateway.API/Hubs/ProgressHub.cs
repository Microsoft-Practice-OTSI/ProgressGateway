using Microsoft.AspNetCore.SignalR;

namespace ProgressGateway.Api.Hubs
{
    /// <summary>
    /// Generic SignalR Hub for progress notifications.
    /// </summary>
    public class ProgressHub : Hub
    {
        /// <summary>
        /// The UI calls this method before starting
        /// the actual business process.
        ///
        /// The browser connection joins an execution group.
        /// </summary>
        public async Task JoinExecutionGroup(string executionId)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                executionId);
        }

        /// <summary>
        /// Optional method to leave the execution group.
        /// </summary>
        public async Task LeaveExecutionGroup(string executionId)
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                executionId);
        }
    }
}