using System.Collections.Generic;

namespace ProgressGateway.UI.Models.Progress
{
    public class ProgressViewModel
    {
        // Unique ID representing one process execution.
        public string ExecutionId { get; set; }

        // Title displayed above the progress bar.
        public string Title { get; set; }

        // Overall progress percentage.
        public int Percentage { get; set; }

        // Current message displayed to the user.
        public string Message { get; set; }

        // Overall process status.
        // Pending, InProgress, Completed, Failed
        public string Status { get; set; }

        // Steps belonging to the current process.
        public List<ProgressStepViewModel> Steps { get; set; }

        public ProgressViewModel()
        {
            Steps = new List<ProgressStepViewModel>();

            Percentage = 0;

            Message = "Waiting to start...";

            Status = "Pending";
        }
    }
}