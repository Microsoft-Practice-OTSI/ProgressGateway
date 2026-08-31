using ProgressGateway.UI.Services;

namespace ProgressGateway.UI.Helpers
{
    public class WorkOrderHelper
    {
        private readonly ProgressGatewayClient _progressGatewayClient;

        private string _currentStep = string.Empty;


        public WorkOrderHelper(
            ProgressGatewayClient progressGatewayClient)
        {
            _progressGatewayClient =
                progressGatewayClient;
        }


        // =====================================================
        // MAIN WORK ORDER PROCESS
        // =====================================================

        public async Task ProcessWorkOrderAsync(
            string executionId)
        {
            try
            {
                // =============================================
                // STEP 1 - PRE VALIDATION
                // =============================================

                _currentStep = "PreValidation";

                await SendProgressAsync(
                    executionId,
                    "InProgress",
                    0,
                    "Pre-validation is in progress..."
                );

                await PreValidationAsync();

                await SendProgressAsync(
                    executionId,
                    "Completed",
                    20,
                    "Pre-validation completed."
                );


                // =============================================
                // STEP 2 - CREATE WORK ORDER
                // =============================================

                _currentStep = "CreateWorkOrder";

                await SendProgressAsync(
                    executionId,
                    "InProgress",
                    20,
                    "Creating work order..."
                );

                await CreateWorkOrderAsync();

                await SendProgressAsync(
                    executionId,
                    "Completed",
                    40,
                    "Work order created successfully."
                );


                // =============================================
                // STEP 3 - CREATE DOCUMENTS
                // =============================================

                _currentStep = "CreateDocuments";

                await SendProgressAsync(
                    executionId,
                    "InProgress",
                    40,
                    "Creating work order documents..."
                );

                await CreateDocumentsAsync();

                await SendProgressAsync(
                    executionId,
                    "Completed",
                    60,
                    "Work order documents created."
                );


                // =============================================
                // STEP 4 - CREATE CONFIRMATIONS
                // =============================================

                _currentStep = "CreateConfirmations";

                await SendProgressAsync(
                    executionId,
                    "InProgress",
                    60,
                    "Creating confirmations..."
                );

                await CreateConfirmationsAsync();

                await SendProgressAsync(
                    executionId,
                    "Completed",
                    80,
                    "Confirmations created."
                );


                // =============================================
                // STEP 5 - GENERATE INVOICE
                // =============================================

                _currentStep = "GenerateInvoice";

                await SendProgressAsync(
                    executionId,
                    "InProgress",
                    80,
                    "Generating invoice..."
                );

                await GenerateInvoiceAsync();

                await SendProgressAsync(
                    executionId,
                    "Completed",
                    100,
                    "Work order process completed successfully."
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


        // =====================================================
        // PROGRESS HELPER METHOD
        //
        // Avoid repeating ProgressGatewayClient code.
        // =====================================================

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


        // =====================================================
        // ACTUAL BUSINESS METHODS
        //
        // Replace Task.Delay with your real application logic.
        // =====================================================

        private async Task PreValidationAsync()
        {
            await Task.Delay(2000);

            Console.WriteLine(
                "Work order pre-validation completed."
            );
        }


        private async Task CreateWorkOrderAsync()
        {
            await Task.Delay(3000);

            Console.WriteLine(
                "Work order created."
            );
        }


        private async Task CreateDocumentsAsync()
        {
            await Task.Delay(2000);

            Console.WriteLine(
                "Documents created."
            );
        }


        private async Task CreateConfirmationsAsync()
        {
            await Task.Delay(2500);

            Console.WriteLine(
                "Confirmations created."
            );
        }


        private async Task GenerateInvoiceAsync()
        {
            await Task.Delay(3000);

            Console.WriteLine(
                "Invoice generated."
            );
        }
    }
}