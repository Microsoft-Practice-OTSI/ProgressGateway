using Microsoft.AspNetCore.Mvc;
using ProgressGateway.UI.Helpers;
using ProgressGateway.UI.Models;

namespace ProgressGateway.UI.Controllers
{
    public class EmployeeOnboardingController : Controller
    {
        private readonly EmployeeOnboardingHelper _employeeOnboardingHelper;

        public EmployeeOnboardingController(
            EmployeeOnboardingHelper employeeOnboardingHelper)
        {
            _employeeOnboardingHelper =
                employeeOnboardingHelper;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


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


            _ = Task.Run(async () =>
            {
                await _employeeOnboardingHelper
                    .ProcessEmployeeOnboardingAsync(
                        request.ExecutionId
                    );
            });


            return Ok(new
            {
                success = true,
                message = "Employee onboarding started."
            });
        }
    }
}