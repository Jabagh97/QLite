using Microsoft.AspNetCore.Mvc;

namespace Quavis.KioskAdmin.WebApp.Controllers
{
    [Route("Account")]
    public class CallbackController : Controller
    {
        [Route("AccessDenied")]
        public IActionResult AccessDenied(string ReturnUrl) => View("Index");
    }
}
