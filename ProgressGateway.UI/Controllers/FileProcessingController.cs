using Microsoft.AspNetCore.Mvc;
using ProgressGateway.UI.Helpers;
using ProgressGateway.UI.Models;

namespace ProgressGateway.UI.Controllers
{
    public class FileProcessingController : Controller
    {
        private readonly FileProcessingHelper _fileProcessingHelper;

        public FileProcessingController(
            FileProcessingHelper fileProcessingHelper)
        {
            _fileProcessingHelper = fileProcessingHelper;
        }

        // =====================================================
        // GET: /FileProcessing
        // =====================================================
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        // =====================================================
        // POST: /FileProcessing/Start
        //
        // Called when user clicks Start File Processing button.
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
                    message = "ExecutionId is required."
                });
            }

            // Start the actual processing in background.
            // Controller returns immediately to browser.
            _ = Task.Run(async () =>
            {
                try
                {
                    await _fileProcessingHelper.RunAsync(
                        request.ExecutionId
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"File processing failed: {ex}"
                    );
                }
            });

            return Ok(new
            {
                success = true,
                message = "File processing started."
            });
        }
    }
}