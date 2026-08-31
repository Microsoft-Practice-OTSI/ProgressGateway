using Microsoft.AspNetCore.Mvc;
using ProgressGateway.UI.Models.Progress;

namespace ProgressGateway.UI.ViewComponents
{
    public class ProgressComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            ProgressViewModel model)
        {
            if (model == null)
            {
                model = new ProgressViewModel();
            }

            return View(model);
        }
    }
}