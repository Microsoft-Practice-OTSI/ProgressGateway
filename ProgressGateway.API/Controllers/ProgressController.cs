using Microsoft.AspNetCore.Mvc;
using ProgressGateway.Api.Models;
using ProgressGateway.Api.Services;

namespace ProgressGateway.Api.Controllers
{
    /// <summary>
    /// Generic API for publishing process progress.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProgressController : ControllerBase
    {
        private readonly IProgressService _progressService;

        public ProgressController(
            IProgressService progressService)
        {
            _progressService = progressService;
        }

        /// <summary>
        /// Any application can call this API
        /// to publish a progress update.
        /// </summary>
        [HttpPost("update")]
        public async Task<IActionResult> Update(
            [FromBody] ProgressUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(
                    request.ExecutionId))
            {
                return BadRequest(
                    new
                    {
                        success = false,
                        message = "ExecutionId is required."
                    });
            }

            if (string.IsNullOrWhiteSpace(
                    request.Step))
            {
                return BadRequest(
                    new
                    {
                        success = false,
                        message = "Step is required."
                    });
            }

            if (string.IsNullOrWhiteSpace(
                    request.Status))
            {
                return BadRequest(
                    new
                    {
                        success = false,
                        message = "Status is required."
                    });
            }

            await _progressService
                .SendProgressUpdateAsync(request);

            return Ok(
                new
                {
                    success = true,
                    message = "Progress update sent successfully."
                });
        }

        /// <summary>
        /// Any application can call this API
        /// when the complete process has finished.
        /// </summary>
        [HttpPost("complete")]
        public async Task<IActionResult> Complete(
            [FromBody] ProgressCompletedRequest request)
        {
            if (string.IsNullOrWhiteSpace(
                    request.ExecutionId))
            {
                return BadRequest(
                    new
                    {
                        success = false,
                        message = "ExecutionId is required."
                    });
            }

            await _progressService
                .SendCompletedAsync(request);

            return Ok(
                new
                {
                    success = true,
                    message = "Process completion sent successfully."
                });
        }
    }
}