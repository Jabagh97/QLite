using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLite.Data.ViewModels;

namespace QLiteAuthenticationServer.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public HomeController(IIdentityServerInteractionService interaction, IWebHostEnvironment environment, ILogger<HomeController> logger, IConfiguration configuration)
        {
            _interaction = interaction;
            _environment = environment;
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                return View();
            else
                return Redirect(_configuration["SiteDomain"] + "/Account/Login");
        }

        /// <summary>
        /// Shows the error page
        /// </summary>
        public async Task<IActionResult> Error(string? errorId, string? customErrorCode)
        {
            var vm = new ErrorViewModel();

            // retrieve error details from identityserver
            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;

                if (!_environment.IsDevelopment())
                {
                    // only show in development
                    message.ErrorDescription = null;
                }
            }

            if (!string.IsNullOrEmpty(customErrorCode))
            {
                vm.simpleErrorMessage = "Error code: " + customErrorCode;
            }

            return View("Error", vm);
        }
    }
}