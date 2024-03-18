using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLite.Data;
using QLite.DesignComponents;
using System.Text;

namespace AdminPortal.Controllers
{
    public class DesignerController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public DesignerController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            var apiBase = _configuration.GetValue<string>("APIBase");
            if (string.IsNullOrEmpty(apiBase))
                throw new ArgumentException("APIBase configuration is missing or invalid.", nameof(apiBase));

            _httpClient.BaseAddress = new Uri(apiBase);
        }

        public async Task<ActionResult> Index(Guid DesignID)
        {
            // Retrieve data for the design page
            DesPageData designData = await GetDesignData(DesignID);

            return View(designData);
        }

        private async Task<DesPageData> GetDesignData(Guid DesignID)
        {
            return await GetDesignResponse<DesPageData>($"api/Admin/GetDesign/{DesignID}");
        }


        [HttpPost]
        [Route("Designer/SaveDesign/{DesignID}")]
        public async Task<IActionResult> SaveDesign(Guid DesignID, [FromBody] DesPageDataViewModel desPageData)
        {
            
            var jsonData = JsonConvert.SerializeObject(new { desPageData.DesPageDataJson });

            var response = await _httpClient.PostAsync($"api/Admin/SaveDesign/{DesignID}", new StringContent(jsonData, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                // Design data saved successfully
                return Ok("Design saved successfully");
            }
            else
            {
                // Failed to save design data
                return StatusCode((int)response.StatusCode, "Failed to save design");
            }
        }


        [HttpGet]
        public Task<IActionResult> GetSegmentList() =>
           GetJsonResponse<List<Segment>>($"api/Admin/GetSegmentList");


        [HttpGet]
        public Task<IActionResult> GetServiceList() =>
           GetJsonResponse<List<Segment>>($"api/Admin/GetServiceList");


        private async Task<IActionResult> GetJsonResponse<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                T deserializedData = JsonConvert.DeserializeObject<T>(responseData);
                return Ok(deserializedData);
            }
            else
            {
                return StatusCode((int)response.StatusCode, $"Failed to retrieve data");
            }
        }
        private async Task<T> GetDesignResponse<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                T deserializedData = JsonConvert.DeserializeObject<T>(responseData);
                return deserializedData;
            }
            else
            {
                return default(T);
            }
        }
    }


   
}
