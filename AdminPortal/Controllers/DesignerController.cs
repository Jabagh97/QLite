using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using QLite.Data;
using QLite.Data.Services;
using QLite.DesignComponents;
using System.Text;

namespace AdminPortal.Controllers
{
    public class DesignerController : Controller
    {
        private readonly ApiService _apiService;

        public DesignerController(ApiService apiService)
        {
            _apiService = apiService;

        }

        public async Task<ActionResult> Index(Guid DesignID)
        {
            // Retrieve data for the design page
            DesPageData designData = await GetDesignData(DesignID);

            return View(designData);
        }

        private async Task<DesPageData> GetDesignData(Guid DesignID)
        {
            return await _apiService.GetDesignResponse<DesPageData>($"api/Admin/GetDesign/{DesignID}");
        }


        [HttpPost]
        [Route("Designer/SaveDesign/{DesignID}")]
        public async Task<IActionResult> SaveDesign(Guid DesignID, [FromBody] DesPageDataViewModel desPageData)
        {
            var response = await _apiService.PostGenericRequest<bool>($"api/Admin/SaveDesign/{DesignID}", desPageData,true);

            if (response)
            {
                return Ok("Design saved successfully");
            }
            else
            {
                return StatusCode(500, "Failed to save design");
            }
        }



        [HttpGet]
        [Route("Designer/GetDesignImageByID/{DesignID}")]

        public async Task<string> GetDesignImageByID(Guid DesignID)
        {
            return await _apiService.GetGenericResponse<string>($"api/Admin/GetDesignImageByID/{DesignID}",true);

        }


        [HttpGet]
        public async Task<List<Design>> GetDesignList()
        {
            return await _apiService.GetGenericResponse<List<Design>>($"api/Admin/GetDesignList");
        }

        [HttpGet]
        public async Task<List<Segment>> GetSegmentList()
        {
            return await _apiService.GetGenericResponse<List<Segment>>($"api/Admin/GetSegmentList");
        }

        [HttpGet]
        public async Task<List<ServiceType>> GetServiceList()
        {
            return await _apiService.GetGenericResponse<List<ServiceType>>($"api/Admin/GetServiceList");
        }
        [HttpGet]
        public async Task<List<Language>> GetLanguageList()
        {
            return await _apiService.GetGenericResponse<List<Language>>($"api/Admin/GetLanguageList");
        }

        public ActionResult Utilities()
        {
            ViewBag.PageTitle = "QR Code Generator";

            return View("QrGenerator");
        }




    }



}
