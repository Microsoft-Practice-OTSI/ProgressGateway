using ProgressGateway.UI.Services;

namespace ProgressGateway.UI.Helpers
{
    public class EmployeeOnboardingHelper
    {
        private readonly ProgressGatewayClient _progressGatewayClient;

        private string _currentStep = string.Empty;


        public EmployeeOnboardingHelper(
            ProgressGatewayClient progressGatewayClient)
        {
            _progressGatewayClient =
                progressGatewayClient;
        }


        public async Task ProcessEmployeeOnboardingAsync(
            string executionId)
        {
            try
            {
                // STEP 1

                _currentStep = "CreateEmployee";

                await SendProgressAsync(
                    executionId,
                    "InProgress",
                    0,
                    "Creating employee record..."
                );

                await CreateEmployeeAsync();

                await SendProgressAsync(
                    executionId,
                    "Completed",
                    25,
                    "Employee record created."
                );


                // STEP 2

                _currentStep = "CreateAccount";

                await SendProgressAsync(
                    executionId,
                    "InProgress",
                    25,
                    "Creating employee account..."
                );

                await CreateAccountAsync();

                await SendProgressAsync(
                    executionId,
                    "Completed",
                    50,
                    "Employee account created."
                );


                // STEP 3

                _currentStep = "AssignEquipment";

                await SendProgressAsync(
                    executionId,
                    "InProgress",
                    50,
                    "Assigning equipment..."
                );

                await AssignEquipmentAsync();

                await SendProgressAsync(
                    executionId,
                    "Completed",
                    75,
                    "Equipment assigned."
                );


                // STEP 4

                _currentStep = "SendWelcomeEmail";

                await SendProgressAsync(
                    executionId,
                    "InProgress",
                    75,
                    "Sending welcome email..."
                );

                await SendWelcomeEmailAsync();

                await SendProgressAsync(
                    executionId,
                    "Completed",
                    100,
                    "Employee onboarding completed successfully."
                );
            }
            catch (Exception ex)
            {
                await SendProgressAsync(
                    executionId,
                    "Failed",
                    0,
                    ex.Message
                );

                throw;
            }
        }


        private async Task SendProgressAsync(
            string executionId,
            string status,
            int percentage,
            string message)
        {
            await _progressGatewayClient
                .UpdateProgressAsync(
                    executionId,
                    _currentStep,
                    status,
                    percentage,
                    message
                );
        }


        private async Task CreateEmployeeAsync()
        {
            await Task.Delay(2000);
        }


        private async Task CreateAccountAsync()
        {
            await Task.Delay(2500);
        }


        private async Task AssignEquipmentAsync()
        {
            await Task.Delay(2500);
        }


        private async Task SendWelcomeEmailAsync()
        {
            await Task.Delay(2000);
        }
    }
}