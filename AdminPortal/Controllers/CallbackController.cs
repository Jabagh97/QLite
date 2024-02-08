using Microsoft.AspNetCore.Mvc;

namespace AdminPortal.Controllers
{
    [Route("Account")]
    public class CallbackController : Controller
    {
        [Route("AccessDenied")]
        public IActionResult AccessDenied(string ReturnUrl) => View("Index");
    }
}
