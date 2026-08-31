using Microsoft.AspNetCore.Mvc;
using ProgressGateway.UI.Helpers;
using ProgressGateway.UI.Models;

namespace ProgressGateway.UI.Controllers
{
    public class ReportGenerationController : Controller
    {
        private readonly ReportGenerationHelper _reportGenerationHelper;

        public ReportGenerationController(
            ReportGenerationHelper reportGenerationHelper)
        {
            _reportGenerationHelper = reportGenerationHelper;
        }

        // =====================================================
        // DISPLAY REPORT GENERATION PAGE
        // =====================================================

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        // =====================================================
        // START REPORT GENERATION
        // =====================================================

        [HttpPost]
        public IActionResult Start(
            [FromBody] StartProcessRequest request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.ExecutionId))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Execution ID is required."
                });
            }

            try
            {
                Console.WriteLine(
                    $"Starting Report Generation. Execution ID: {request.ExecutionId}"
                );

                // -------------------------------------------------
                // Run report generation in the background.
                //
                // The HTTP request immediately returns success.
                // Progress updates will be sent through the API
                // and received through SignalR.
                // -------------------------------------------------

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _reportGenerationHelper
                            .GenerateReportAsync(
                                request.ExecutionId
                            );
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(
                            $"Report Generation Error: {ex.Message}"
                        );
                    }
                });

                return Ok(new
                {
                    success = true,
                    message = "Report generation started."
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error starting report generation: {ex.Message}"
                );

                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}