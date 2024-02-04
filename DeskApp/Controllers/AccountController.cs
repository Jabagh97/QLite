using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Quavis.KioskAdmin.WebApp.Controllers
{
    [Authorize] 
    public class AccountController : Controller
    {
        private readonly IConfiguration configuration;

        public AccountController(IConfiguration configuration)
            =>
            this.configuration = configuration;

        public async Task<IActionResult> Login()
        {
            return View("Views/Home/Index.cshtml");

        }

        public async Task<IActionResult> Logout()
        {


            await HttpContext.SignOutAsync("cookies");
            await HttpContext.SignOutAsync("oidc");
            return View("Views/Home/Index.cshtml");

        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/Account/InvalidFingerprint")]
        public async Task<IActionResult> InvalidFingerprint()
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync("cookies");
                await HttpContext.SignOutAsync("oidc");
            }
            return View("/Views/Callback/InvalidFingerprint.cshtml");
        }
    }
}
