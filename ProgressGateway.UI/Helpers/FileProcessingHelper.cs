using ProgressGateway.UI.Services;

namespace ProgressGateway.UI.Helpers
{
    public class FileProcessingHelper
    {
        private readonly ProgressGatewayClient
            _progressGatewayClient;

        public FileProcessingHelper(
            ProgressGatewayClient progressGatewayClient)
        {
            _progressGatewayClient =
                progressGatewayClient;
        }


        // =====================================================
        // MAIN PROCESS
        // =====================================================

        public async Task RunAsync(
            string executionId)
        {
            string currentStep = string.Empty;

            try
            {
                // =================================================
                // STEP 1 - VALIDATE FILE
                // =================================================

                currentStep = "ValidateFile";

                await SendProgressAsync(
                    executionId,
                    currentStep,
                    "InProgress",
                    0,
                    "File validation is in progress..."
                );

                await ValidateFileAsync();

                await SendProgressAsync(
                    executionId,
                    currentStep,
                    "Completed",
                    25,
                    "File validation completed."
                );


                // =================================================
                // STEP 2 - UPLOAD FILE
                // =================================================

                currentStep = "UploadFile";

                await SendProgressAsync(
                    executionId,
                    currentStep,
                    "InProgress",
                    25,
                    "File upload is in progress..."
                );

                await UploadFileAsync();

                await SendProgressAsync(
                    executionId,
                    currentStep,
                    "Completed",
                    50,
                    "File uploaded successfully."
                );


                // =================================================
                // STEP 3 - PROCESS FILE
                // =================================================

                currentStep = "ProcessFile";

                await SendProgressAsync(
                    executionId,
                    currentStep,
                    "InProgress",
                    50,
                    "File processing is in progress..."
                );

                await ProcessUploadedFileAsync();

                await SendProgressAsync(
                    executionId,
                    currentStep,
                    "Completed",
                    75,
                    "File processing completed."
                );


                // =================================================
                // STEP 4 - SAVE RESULT
                // =================================================

                currentStep = "SaveResult";

                await SendProgressAsync(
                    executionId,
                    currentStep,
                    "InProgress",
                    75,
                    "Saving processing results..."
                );

                await SaveResultAsync();

                await SendProgressAsync(
                    executionId,
                    currentStep,
                    "Completed",
                    100,
                    "File processing completed successfully."
                );
            }
            catch (Exception ex)
            {
                await SendProgressAsync(
                    executionId,
                    currentStep,
                    "Failed",
                    0,
                    ex.Message
                );

                throw;
            }
        }


        // =====================================================
        // SEND UPDATE TO PROGRESS GATEWAY API
        // =====================================================

        private async Task SendProgressAsync(
            string executionId,
            string stepName,
            string status,
            int percentage,
            string message)
        {
            await _progressGatewayClient
                .UpdateProgressAsync(
                    executionId,
                    stepName,
                    status,
                    percentage,
                    message
                );
        }


        // =====================================================
        // ACTUAL BUSINESS METHODS
        //
        // Replace these Task.Delay calls with real logic later.
        // =====================================================

        private async Task ValidateFileAsync()
        {
            await Task.Delay(2000);

            Console.WriteLine(
                "File validation completed"
            );
        }


        private async Task UploadFileAsync()
        {
            await Task.Delay(3000);

            Console.WriteLine(
                "File uploaded successfully"
            );
        }


        private async Task ProcessUploadedFileAsync()
        {
            await Task.Delay(3000);

            Console.WriteLine(
                "File processing completed"
            );
        }


        private async Task SaveResultAsync()
        {
            await Task.Delay(2000);

            Console.WriteLine(
                "Results saved"
            );
        }
    }
}