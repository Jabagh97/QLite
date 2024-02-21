using Microsoft.AspNetCore.Mvc;
using QLite.Data;

namespace KioskApp.Controllers
{
    public class SegmentController : Controller
    {
        public IActionResult Index()
        {
            var segments = FetchSegmentsFromAPI(); 
            ViewBag.Segments = segments;

         
            return PartialView("Views/Home/Segments.cshtml");
        }

        private List<Segment> FetchSegmentsFromAPI()
        {

            return new List<Segment>
        {
            new Segment { Oid = new Guid() , Name = "Segment 1" },
            new Segment { Oid = new Guid(), Name = "Segment 2" },
            new Segment { Oid = new Guid(), Name = "Segment 3" },
        };
        }
    }
}
