namespace ProgressGateway.Api.Models
{
    /// <summary>
    /// Generic request used by any application
    /// to send a progress update.
    /// </summary>
    public class ProgressUpdateRequest
    {
        /// <summary>
        /// Unique ID representing one execution/process.
        /// </summary>
        public string ExecutionId { get; set; }

        /// <summary>
        /// Generic step name.
        ///
        /// Examples:
        /// PreValidation
        /// CreateEmployee
        /// UploadFile
        /// GenerateInvoice
        /// </summary>
        public string Step { get; set; }

        /// <summary>
        /// Pending
        /// InProgress
        /// Completed
        /// Failed
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Optional message to display to the user.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Optional percentage.
        ///
        /// Example:
        /// 20
        /// 40
        /// 60
        /// 100
        /// </summary>
        public int Percentage { get; set; }
    }
}