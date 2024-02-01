using AdminPortal.Constants;
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
            _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5169/") };
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

                var response = await _httpClient.GetAsync(EndPoints.AdminGetData(modelName, parameters.SearchValue, parameters.SortColumn, parameters.SortColumnDirection, parameters.Skip, parameters.PageSize));
                return response.IsSuccessStatusCode
                    ? Ok(await response.Content.ReadAsStringAsync())
                    : StatusCode(500, new { success = false, message = "Internal Server Error" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"{ex.Message} Internal Server Error" });
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] Dictionary<string, object> formData) =>
            ProcessData(EndPoints.AdminCreate, formData, "Item created successfully", "Error creating");

        [HttpPost]
        public IActionResult Edit([FromBody] Dictionary<string, object> formData) =>
            ProcessData(EndPoints.AdminEdit, formData, "Item edited successfully", "Error editing");

        [HttpPost]
        public IActionResult Delete([FromBody] Dictionary<string, object> formData) =>
            ProcessData(EndPoints.AdminDelete, formData, "Item deleted successfully", "Error deleting");

        public async Task<IActionResult> ShowPopup(string modelName, string opType, Dictionary<string, string> data)
        {
            try
            {
                if (!_modelTypeMappingService.TryGetModelTypes(modelName, out var modelType, out var viewModelType))
                {
                    return NotFound(Errors.NullModel);
                }

                var response = await _httpClient.GetAsync(EndPoints.GetdropDowns(modelName));

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.DropDowns = JsonConvert.DeserializeObject<Dictionary<string, List<dynamic>>>(await response.Content.ReadAsStringAsync());
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
                return NotFound($" '{modelName}' {Errors.ModelNotFound}  {ex.Message}");
            }
        }



        public IActionResult ProcessData(string apiUrl, Dictionary<string, object> formData, string successMessage, string errorMessage)
        {
            try
            {
                var convertedFormData = formData.ToDictionary(
                    kvp => kvp.Key,
                    kvp => Utils.ConvertJsonElementValue((JsonElement)kvp.Value));

                var jsonFormData = JsonConvert.SerializeObject(convertedFormData);

                var content = new StringContent(jsonFormData, Encoding.UTF8, "application/json");
                var response = _httpClient.PostAsync(apiUrl, content).Result;

                return response.IsSuccessStatusCode
                    ? Ok(new { success = true, message = successMessage })
                    : StatusCode((int)response.StatusCode, new { success = false, message = errorMessage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }


    }

}
