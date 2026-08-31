using ProgressGateway.Api.Models;

namespace ProgressGateway.Api.Services
{
    public interface IProgressService
    {
        Task SendProgressUpdateAsync(
            ProgressUpdateRequest request);

        Task SendCompletedAsync(
            ProgressCompletedRequest request);
    }
}