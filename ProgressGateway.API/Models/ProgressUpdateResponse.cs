namespace ProgressGateway.Api.Models
{
    /// <summary>
    /// Standard response returned by the Progress Gateway API.
    /// </summary>
    public class ProgressUpdateResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; }
    }
}