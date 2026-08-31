using Microsoft.AspNetCore.SignalR;
using ProgressGateway.Api.Hubs;
using ProgressGateway.Api.Models;

namespace ProgressGateway.Api.Services
{
    /// <summary>
    /// Generic service responsible for broadcasting
    /// progress updates to SignalR clients.
    /// </summary>
    public class ProgressService : IProgressService
    {
        private readonly IHubContext<ProgressHub> _hubContext;

        public ProgressService(
            IHubContext<ProgressHub> hubContext)
        {
            _hubContext = hubContext;
        }

        /// <summary>
        /// Sends a progress update to all clients
        /// connected to the specified execution group.
        /// </summary>
        public async Task SendProgressUpdateAsync(
            ProgressUpdateRequest request)
        {
            await _hubContext
                .Clients
                .Group(request.ExecutionId)
                .SendAsync(
                    "ReceiveProgressUpdate",
                    request);
        }

        /// <summary>
        /// Sends a completion notification to all clients
        /// connected to the specified execution group.
        /// </summary>
        public async Task SendCompletedAsync(
            ProgressCompletedRequest request)
        {
            await _hubContext
                .Clients
                .Group(request.ExecutionId)
                .SendAsync(
                    "ReceiveProgressCompleted",
                    request);
        }
    }
}