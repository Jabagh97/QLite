using DeskApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DeskApp.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        public IActionResult Index()
            => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string? errorCode)
            =>
            View(new ErrorViewModel
            {
                RequestId = string.IsNullOrEmpty(errorCode) ? Activity.Current?.Id ?? HttpContext.TraceIdentifier : errorCode
            });
    }
}