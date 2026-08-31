using Microsoft.AspNetCore.Mvc;
using ProgressGateway.UI.Helpers;

namespace ProgressGateway.UI.Controllers
{
    public class WorkOrderController : Controller
    {
        private readonly WorkOrderHelper _workOrderHelper;

        public WorkOrderController(
            WorkOrderHelper workOrderHelper)
        {
            _workOrderHelper = workOrderHelper;
        }

        // =====================================================
        // DISPLAY WORK ORDER PAGE
        // =====================================================

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        // =====================================================
        // START WORK ORDER PROCESS
        //
        // Called from Index.cshtml
        // =====================================================

        [HttpPost]
        public async Task<IActionResult> Start(
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
                // ---------------------------------------------
                // Start process in background.
                //
                // Controller immediately returns success.
                // Progress updates continue through SignalR.
                // ---------------------------------------------

                _ = Task.Run(async () =>
                {
                    await _workOrderHelper
                        .ProcessWorkOrderAsync(
                            request.ExecutionId
                        );
                });


                return Ok(new
                {
                    success = true,
                    message = "Work order process started."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }


    // =========================================================
    // GENERIC REQUEST MODEL
    //
    // You can move this later to Models folder.
    // =========================================================

    public class StartProcessRequest
    {
        public string ExecutionId { get; set; }
    }
}