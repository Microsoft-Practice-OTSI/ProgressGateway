namespace ProgressGateway.UI.Models.Progress
{
    public class ProgressStepViewModel
    {
        // Unique/technical name.
        // This must match the StepName received from ProgressGateway.Api.
        public string StepName { get; set; }

        // Friendly name displayed in the UI.
        public string DisplayName { get; set; }

        // Pending, InProgress, Completed, Failed
        public string Status { get; set; }

        // Optional message related to this step.
        public string Message { get; set; }

        public ProgressStepViewModel()
        {
            Status = "Pending";
        }
    }
}