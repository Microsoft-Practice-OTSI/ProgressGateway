namespace ProgressGateway.Api.Models
{
    /// <summary>
    /// Sent when an entire process is completed.
    /// </summary>
    public class ProgressCompletedRequest
    {
        /// <summary>
        /// Unique execution ID.
        /// </summary>
        public string ExecutionId { get; set; }

        /// <summary>
        /// Optional completion message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Optional result ID.
        ///
        /// For example:
        /// WorkOrderId
        /// EmployeeId
        /// FileId
        /// ReportId
        /// </summary>
        public string ResultId { get; set; }
    }
}