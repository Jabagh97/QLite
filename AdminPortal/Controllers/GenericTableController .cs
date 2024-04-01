using AdminPortal.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PortalPOC.Helpers;
using QLite.Data;
using QLite.Data.Services;
using QLite.DesignComponents;
using QLiteDataApi.Constants;
using System.Text;
using System.Text.Json;

namespace PortalPOC.Controllers
{

    /// <summary>
    /// Controller for handling generic table operations such as data fetching,
    /// creation, editing, and deletion of items based on dynamic model names.
    /// </summary>
    [Authorize]

    public class GenericTableController : Controller
    {
        private readonly IModelTypeMappingService _modelTypeMappingService;
        private readonly IDataTableRequestExtractor _requestExtractor;
        private readonly ApiService _apiService;


        public GenericTableController(IModelTypeMappingService modelTypeMappingService, IDataTableRequestExtractor requestExtractor, ApiService apiService)
        {
            _modelTypeMappingService = modelTypeMappingService;
            _requestExtractor = requestExtractor;
            _apiService = apiService;

        }

        /// <summary>
        /// Serves the index view for a given model name, dynamically setting the model type.
        /// </summary>
        /// <param name="modelName">The name of the model for which to display the table.</param>
        /// <returns>The index view for the specified model or an error view if the model type is not found.</returns>
        public IActionResult Index(string modelName)
        {
            if (!_modelTypeMappingService.TryGetModelTypes(modelName, out var modelType, out var viewModelType))
            {
                return View("Error");
            }

            ViewBag.PageTitle = modelName;
            return View(ViewNavigations.GenericTable, viewModelType);
        }

        /// <summary>
        /// Fetches data for the specified model name using the ApiService.
        /// </summary>
        /// <param name="modelName">The name of the model for which to fetch data.</param>
        /// <returns>A JSON result containing the fetched data.</returns>
        [HttpPost]
        public async Task<IActionResult> GetData(string modelName)
        {
            var result = await _apiService.GetGenericResponse<string>(EndPoints.AdminGetData(modelName), true);

            return Ok(result);
        }

        /// <summary>
        /// Processes a creation request for the specified model data.
        /// </summary>
        /// <param name="formData">The form data to process.</param>
        /// <returns>A JSON result indicating success or failure.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Dictionary<string, object> formData) =>
           await ProcessData(EndPoints.AdminCreate, formData, "Item created successfully", "Error creating");

        /// <summary>
        /// Processes an edit request for the specified model data.
        /// </summary>
        /// <param name="formData">The form data to process.</param>
        /// <returns>A JSON result indicating success or failure.</returns>
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] Dictionary<string, object> formData) =>
           await ProcessData(EndPoints.AdminEdit, formData, "Item edited successfully", "Error editing");

        /// <summary>
        /// Processes a deletion request for the specified model data.
        /// </summary>
        /// <param name="formData">The form data to process.</param>
        /// <returns>A JSON result indicating success or failure.</returns>
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] Dictionary<string, object> formData) =>
           await ProcessData(EndPoints.AdminDelete, formData, "Item deleted successfully", "Error deleting");


        /// <summary>
        /// Displays a popup for the specified operation type on the given model.
        /// </summary>
        /// <param name="modelName">The model name.</param>
        /// <param name="opType">The operation type (e.g., Edit).</param>
        /// <param name="data">Additional data to display in the popup.</param>
        /// <returns>A PartialView result for displaying the popup.</returns>
        public async Task<IActionResult> ShowPopup(string modelName, string opType, Dictionary<string, string> data)
        {
            try
            {
                if (!_modelTypeMappingService.TryGetModelTypes(modelName, out var modelType, out var viewModelType))
                {
                    return NotFound(Errors.NullModel);
                }
                ViewBag.DropDowns = await _apiService.GetGenericResponse<Dictionary<string, List<dynamic>>>(EndPoints.GetdropDowns(modelName));

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

        /// <summary>
        /// Processes generic data operations (Create, Edit, Delete) based on the given apiUrl.
        /// </summary>
        /// <param name="apiUrl">The API endpoint to process the request.</param>
        /// <param name="formData">The form data for the operation.</param>
        /// <param name="successMessage">The success message to return if the operation is successful.</param>
        /// <param name="errorMessage">The error message to return if the operation fails.</param>
        /// <returns>A JSON result indicating success or failure of the operation.</returns>
        public async Task<IActionResult> ProcessData(string apiUrl, Dictionary<string, object> formData, string successMessage, string errorMessage)
        {
            try
            {
                var convertedFormData = formData.ToDictionary(
                    kvp => kvp.Key,
                    kvp => Utils.ConvertJsonElementValue((JsonElement)kvp.Value));

                var response = await _apiService.PostGenericRequest<bool>(apiUrl, convertedFormData, true);



                return response
                    ? Ok(new { success = true, message = successMessage })
                    : StatusCode(500, new { success = false, message = errorMessage });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Loads tab data for the specified modelName and operation type.
        /// </summary>
        /// <param name="tabName">The name of the tab to load data for.</param>
        /// <param name="modelName">The name of the model associated with the tab.</param>
        /// <param name="Oid">The identifier of the item to load data for.</param>
        /// <returns>A JSON result containing the loaded data.</returns>
        [HttpPost]
        public async Task<ActionResult> LoadTabData(string tabName, string modelName, string Oid)
        {
            try
            {
                var response = await _apiService.GetGenericResponse<string>(EndPoints.AdminGetCollection(tabName, modelName, Oid), true);


                return Ok(new { status = "success", data = response });

            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes selected rows based on the provided request.
        /// </summary>
        /// <param name="request">The request containing information about the rows to delete.</param>
        /// <returns>A JSON result indicating success or failure of the operation.</returns>
        [HttpPost]
        public async Task<ActionResult> DeleteSelectedRows([FromBody] DeleteRowsRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Invalid request parameters.");
                }

                var response = await _apiService.PostGenericRequest<object>(EndPoints.AdminDeleteFromCollection, request);
                return Json(response);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it according to your requirements
                return Json(new { status = "error", error = ex.Message });
            }
        }




    }

}
