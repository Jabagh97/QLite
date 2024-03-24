using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLiteDataApi.Constants;
using System.Xml.Serialization;

namespace AdminPortal.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index(string reportType = "")
        {
            ViewBag.PageTitle = reportType;
            return View(reportType);
        }


        [HttpPost]
        public IActionResult GenerateReport(string reportType)
        {
        return Ok(reportType);
        }
    }
}
