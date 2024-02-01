using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using PortalPOC.Helpers;
using QLite.Data.Services;
using QLiteDataApi.Constants;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace PortalPOC.Controllers
{
    public class GenericTableController : Controller
    {
        private readonly IModelTypeMappingService _modelTypeMappingService;
        private readonly IDataTableRequestExtractor _requestExtractor;

        private readonly HttpClient _httpClient;


        public GenericTableController(IModelTypeMappingService modelTypeMappingService, IDataTableRequestExtractor requestExtractor)
        {
            _modelTypeMappingService = modelTypeMappingService;
            _requestExtractor = requestExtractor;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5169/");
        }

        public IActionResult Index(string modelName)
        {
            if (!_modelTypeMappingService.TryGetModelTypes(modelName, out var modelType, out var viewModelType))
            {
                return View("Error");
            }

            ViewBag.PageTitle = modelName;


            return View(ViewNavigations.GenericTable, viewModelType);
        }


        [HttpPost]
        public async Task<IActionResult> GetData(string modelName)
        {
            try
            {
                var parameters = _requestExtractor.ExtractParameters(Request.Form);

                var apiUrl = $"api/Admin/GetData?modelName={modelName}&searchValue={parameters.SearchValue}&sortColumn={parameters.SortColumn}&sortColumnDirection={parameters.SortColumnDirection}&skip={parameters.Skip}&pageSize={parameters.PageSize}";

                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    return Ok(jsonData);
                }
                else
                {
                    return StatusCode(500, new { success = false, message = "Internal Server Error" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message + " Internal Server Error" });
            }
        }



        [HttpPost]
        public IActionResult Create([FromBody] Dictionary<string, object> formData)
        {
            try
            {
                var apiUrl = "api/Admin/Create";


                var convertedFormData = formData.ToDictionary(
                    kvp => kvp.Key,
                    kvp => Utils.ConvertJsonElementValue((JsonElement)kvp.Value));

                var jsonFormData = JsonConvert.SerializeObject(convertedFormData);


                // Create HttpContent with JSON data
                var content = new StringContent(jsonFormData, Encoding.UTF8, "application/json");

                // Make the request to the second API method
                var response = _httpClient.PostAsync(apiUrl, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    // Process the response if needed
                    var result = response.Content.ReadAsStringAsync().Result;
                    return Ok(result);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, new { success = false, message = "Error creating" });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
      




        [HttpPost]
        public IActionResult Edit([FromBody] Dictionary<string, object> formData)
        {
            try
            {
                try
                {
                    var apiUrl = "api/Admin/Edit";


                    var convertedFormData = formData.ToDictionary(
                        kvp => kvp.Key,
                        kvp => Utils.ConvertJsonElementValue((JsonElement)kvp.Value));

                    var jsonFormData = JsonConvert.SerializeObject(convertedFormData);


                    // Create HttpContent with JSON data
                    var content = new StringContent(jsonFormData, Encoding.UTF8, "application/json");

                    // Make the request to the second API method
                    var response = _httpClient.PostAsync(apiUrl, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        // Process the response if needed
                        var result = response.Content.ReadAsStringAsync().Result;
                        return Ok(result);
                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, new { success = false, message = "Error Editing" });
                    }

                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { success = false, message = ex.Message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        public IActionResult Delete([FromBody] Dictionary<string, object> Data)
        {
            try
            {
                var apiUrl = "api/Admin/Delete";


                var convertedFormData = Data.ToDictionary(
                    kvp => kvp.Key,
                    kvp => Utils.ConvertJsonElementValue((JsonElement)kvp.Value));

                var jsonFormData = JsonConvert.SerializeObject(convertedFormData);


                // Create HttpContent with JSON data
                var content = new StringContent(jsonFormData, Encoding.UTF8, "application/json");

                // Make the request to the second API method
                var response = _httpClient.PostAsync(apiUrl, content).Result;


                // Return a success response with the created model
                return Ok(new { success = true, message = "item Deleted successfully" });
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an error response
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }


        public async Task<IActionResult> ShowPopup(string modelName, string opType, Dictionary<string, string> data)
        {
            try
            {
                var modelTypeMapping = _modelTypeMappingService.GetModelTypeMapping();


                if (!_modelTypeMappingService.TryGetModelTypes(modelName, out var modelType, out var viewModelType))
                {
                    return NotFound(Errors.NullModel);
                }


                var apiUrl = $"api/Admin/GetDropDowns?modelName={modelName}";

                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var namesDictionaryJson = await response.Content.ReadAsStringAsync();
                    var namesDictionary = JsonConvert.DeserializeObject<Dictionary<string, List<dynamic>>>(namesDictionaryJson);

                    ViewBag.DropDowns = namesDictionary;

                }
                else
                {
                    return StatusCode((int)response.StatusCode, new { success = false, message = "PopUp Error" });
                }


                if (!opType.IsNullOrEmpty() && opType.Contains(Operations.Edit))
                {
                    ViewBag.Data = data;
                }

                ViewBag.ViewModel = viewModelType;
                ViewBag.Action = opType;


                return PartialView(ViewNavigations.GenericPartial, modelType);

            }
            catch (Exception ex)
            {
                // Log the exception
                return NotFound($" '{modelName}' {Errors.ModelNotFound}  {ex.Message}");
            }
        }



    }

}
