using Microsoft.AspNetCore.Mvc;

namespace BankManagementSystemVersionFinal1.Controllers
{

    [Route("management")]
    public class ManagementController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/ManagementView.cshtml");
        }
    }
}
