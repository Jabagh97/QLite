using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLite.Data;
using QLite.Data.Services;
using QLite.DesignComponents;
using QLiteDataApi.Constants;
using QLiteDataApi.Helpers;
using QLiteDataApi.Services;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace QLiteDataApi.Controllers
{
    /// <summary>
    /// Manages administrative actions including CRUD operations on models, fetching dropdown data,
    /// handling designs and reports, and other administrative tasks.
    /// </summary>
    [ApiController]
    [Route("api/Admin")]
    public class AdminController : Controller
    {
        private readonly IModelTypeMappingService _modelTypeMappingService;
        private readonly AdminService _dataService;

        public AdminController(AdminService dataService, IModelTypeMappingService modelTypeMappingService)
        {
            _dataService = dataService;
            _modelTypeMappingService = modelTypeMappingService;
        }

        /// <summary>
        /// Retrieves filtered and paginated data for a specified model.
        /// </summary>
        /// <param name="modelName">The name of the model to fetch data for.</param>
        /// <returns>A paginated list of model data.</returns>
        [HttpGet]
        [Route("GetData")]
        public IActionResult GetData(string modelName)
        {
            try
            {
                if (!_modelTypeMappingService.TryGetModelTypes(modelName, out var modelType, out var viewModelType))
                {
                    return NotFound(Errors.NullModel);
                }

                var filteredData = _dataService.GetFilteredAndPaginatedData(modelType, viewModelType);
                var paginatedData = filteredData.ToDynamicList();
                var recordsTotal = filteredData.Count();
                var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = paginatedData };

                // Return the filtered data
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message + " Internal Server Error" });
            }
        }


        /// <summary>
        /// Creates a new instance of a model with the specified data.
        /// </summary>
        /// <param name="formData">A dictionary containing the data for the new model instance.</param>
        /// <returns>A success message if the model is created successfully; otherwise, an error message.</returns>

        [HttpPost]
        [Route("Create")]
        public IActionResult Create([FromBody] Dictionary<string, object> formData)
        {
            try
            {
                return ProcessModelOperation(formData, isCreateOperation: true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing model instance with the specified data.
        /// </summary>
        /// <param name="formData">A dictionary containing the updated data for the model instance.</param>
        /// <returns>A success message if the model is updated successfully; otherwise, an error message.</returns>

        [HttpPost]
        [Route("Edit")]

        public IActionResult Edit([FromBody] Dictionary<string, object> formData)
        {
            try
            {
                return ProcessModelOperation(formData, isCreateOperation: false);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }


        /// <summary>
        /// Retrieves dropdown list data for a specified model.
        /// </summary>
        /// <param name="modelName">The name of the model to retrieve dropdown data for.</param>
        /// <returns>A dictionary containing dropdown list data.</returns>

        [HttpGet]
        [Route("GetDropDowns")]
        public IActionResult GetDropDowns(string modelName)
        {
            try
            {
                var modelTypeMapping = _modelTypeMappingService.GetModelTypeMapping();


                if (!_modelTypeMappingService.TryGetModelTypes(modelName, out var modelType, out var viewModelType))
                {
                    return NotFound(Errors.NullModel);
                }
                Dictionary<string, List<dynamic>> namesDictionary = _dataService.GetGuidPropertyNames(modelType, modelTypeMapping);

                return Ok(namesDictionary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a model instance based on the provided data.
        /// </summary>
        /// <param name="Data">A dictionary containing data to identify the model instance to delete.</param>
        /// <returns>A success message if the model is deleted successfully; otherwise, an error message.</returns>

        [HttpPost]
        [Route("Delete")]

        public IActionResult Delete([FromBody] Dictionary<string, object> Data)
        {
            try
            {
                // Validate Data and create a model instance
                if (!Data.ContainsKey("modelType"))
                {
                    return BadRequest(Errors.NoModelType);
                }

                // Extract modelType from formData
                string modelName = Data["modelType"].ToString();

                if (!_modelTypeMappingService.TryGetModelTypes(modelName, out var modelType, out var viewModelType))
                {
                    return NotFound(Errors.NullModel);
                }


                Data.Remove("modelType");

                // Create an instance of the model using the specified modelType
                var modelInstance = _dataService.SoftDelete(modelType, Data);

                // Return a success response with the created model
                return Ok(new { success = modelInstance, message = "Model Deleted successfully" });
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an error response
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Loads data for a specific tab within a model's detail view, typically for related collections.
        /// </summary>
        /// <param name="tabName">The name of the tab or the related property name in the model.</param>
        /// <param name="modelName">The name of the model to which the tab belongs.</param>
        /// <param name="Oid">The unique identifier (object id) of the model instance.</param>
        /// <returns>A collection of data related to the specified tab or an error message if the operation fails.</returns>
        [HttpGet]
        [Route("GetDbSetCollection")]
        public ActionResult LoadTabData(string tabName, string modelName, string Oid)
        {
            try
            {
                var modelTypeMapping = _modelTypeMappingService.GetModelTypeMapping();


                if (!_modelTypeMappingService.TryGetModelTypes(modelName, out var modelType, out var viewModelType))
                {
                    return NotFound($" '{modelName}' {Errors.ModelNotFound} ");
                }

                PropertyInfo tabProperty = modelType.GetProperty(tabName);

                if (tabProperty?.PropertyType.IsGenericType == true &&
                    tabProperty.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    // Get the inner type of the ICollection<T>
                    Type innerType = tabProperty.PropertyType.GetGenericArguments()[0];

                    // Find matching viewModelType using LINQ
                    Type tabViewModelType = modelTypeMapping.FirstOrDefault(pair => pair.Value.Item1 == innerType).Value.Item2;

                    if (tabViewModelType != null)
                    {
                        var data = _dataService.GetTabData(innerType, tabViewModelType, Oid, modelType).ToDynamicList();
                        return Ok(new { data });
                    }
                    else
                    {
                        return BadRequest($" {Errors.NoMatchingViewModel}  '{innerType}'.");
                    }
                }
                else
                {
                    return BadRequest($"Property '{tabName}' {Errors.NotCollection}");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions if needed
                return Json(new { status = "error", message = ex.Message });
            }
        }


        /// <summary>
        /// Deletes selected rows from a DbSet collection, typically used for removing items from related collections.
        /// </summary>
        /// <param name="request">The request containing the tab name, model name, model OID, and selected OIDs for deletion.</param>
        /// <returns>A success message if rows are deleted successfully; otherwise, an error message.</returns>
        [HttpPost]
        [Route("DeleteFromDbSetCollection")]

        public ActionResult DeleteSelectedRows([FromBody] DeleteRowsRequest request)
        {
            try
            {
                // Check for null values
                if (request == null || request.SelectedOids == null || string.IsNullOrEmpty(request.TabName) || string.IsNullOrEmpty(request.ModelName))
                {
                    return BadRequest("Invalid request parameters.");
                }

                var selectedOids = request.SelectedOids;
                var tabName = request.TabName;
                var modelName = request.ModelName;

                var modelOid = request.ModelOid;

                // Check if modelType and viewModelType can be retrieved
                if (!_modelTypeMappingService.TryGetModelTypes(modelName, out var modelType, out var viewModelType))
                {
                    return NotFound($" '{modelName}' {Errors.ModelNotFound} ");
                }

                // Perform deletion logic
                var result = _dataService.RemoveFromSubList(tabName, modelType, modelOid, selectedOids);

                if (result)
                {
                    return Ok(new { status = "success" });
                }
                else
                {
                    return BadRequest("Failed to delete rows.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it according to your requirements
                return Json(new { status = "error", error = ex.Message });
            }
        }



        /// <summary>
        /// Retrieves the design data for a specific design ID.
        /// </summary>
        /// <param name="DesignID">The unique identifier for the design.</param>
        /// <returns>The design data if found; otherwise, a NotFound result.</returns>
        [HttpGet]
        [Route("GetDesign/{DesignID}")]
        public IActionResult GetDesign(Guid DesignID)
        {
            var design = _dataService.GetDesign(DesignID);

            if (design != null)
            {
                return Ok(design.DesignData);
            }

            return NotFound();
        }

        /// <summary>
        /// Saves or updates the design data for a given design ID.
        /// </summary>
        /// <param name="DesignID">The unique identifier for the design to be saved or updated.</param>
        /// <param name="desPageData">The design page data view model containing the JSON representation of the design and possibly an image.</param>
        /// <returns>A success result if the design is saved; otherwise, a NotFound result.</returns>
        [HttpPost]
        [Route("SaveDesign/{DesignID}")]
        public IActionResult SaveDesign(Guid DesignID, [FromBody] DesPageDataViewModel desPageData)
        {
            // Deserialize DesPageDataJson into DesPageData
            DesPageData desPageDataTest = JsonConvert.DeserializeObject<DesPageData>(desPageData.DesPageDataJson);



            // You can use desPageData.DesignImage here as needed

            var saved = _dataService.SaveDesign(DesignID, desPageDataTest, desPageData.DesignImage);

            if (saved)
            {
                return Ok(true);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Retrieves a list of all desks/designs/services/segments/languages.
        /// </summary>
        /// <returns>A list of desks/designs/services/segments/languages.</returns>

        [HttpGet("GetDesignList")]
        public async Task<IActionResult> GetDesignList()
        {
            var designs = await _dataService.GetDesignList();
            return Ok(designs);
        }
        [HttpGet("GetServiceList")]
        public async Task<IActionResult> GetServiceList()
        {
            var services = await _dataService.GetServiceList();
            return Ok(services);
        }
        [HttpGet("GetSegmentList")]
        public async Task<IActionResult> GetSegmentList()
        {
            var segments = await _dataService.GetSegmentList();
            return Ok(segments);
        }
        [HttpGet("GetLanguageList")]
        public async Task<IActionResult> GetLanguageList()
        {
            var segments = await _dataService.GetLanguageList();
            return Ok(segments);
        }
        [HttpGet]
        [Route("GetAllDesks")]
        public IActionResult GetAllDesks()
        {
            var DeskList = _dataService.GetAllDesks();

            return Ok(DeskList);
        }
        /// <summary>
        /// Retrieves the image for a specific design by its ID.
        /// </summary>
        /// <param name="DesignID">The unique identifier of the design.</param>
        /// <returns>The design image as a base64 string.</returns>

        [HttpGet]
        [Route("GetDesignImageByID/{DesignID}")]
        public async Task<IActionResult> GetDesignImageByID(Guid DesignID)
        {
            string designImage = await _dataService.GetDesignImageByID(DesignID);


            return Ok(designImage);
        }

        /// <summary>
        /// Generates a report of ticket states within a specified date range.
        /// </summary>
        /// <param name="StartDate">The start date of the report period.</param>
        /// <param name="EndDate">The end date of the report period.</param>
        /// <returns>A report of ticket states.</returns>

        [HttpGet]
        [Route("GetTicketStateReport/{StartDate}/{EndDate}")]
        public async Task<IActionResult> GetTicketStateReport(DateTime StartDate, DateTime EndDate)
        {
            string reportData = await _dataService.GetTicketStateReport(StartDate, EndDate);

            return Ok(reportData);
        }

        #region Helpers

        private IActionResult ProcessModelOperation(Dictionary<string, object> formData, bool isCreateOperation)
        {
            // Validate formData and create/update a model instance
            if (!formData.ContainsKey("modelType"))
            {
                return BadRequest(Errors.NoModelType);
            }

            // Extract modelType from formData
            string modelName = formData["modelType"].ToString();


            if (!_modelTypeMappingService.TryGetModelTypes(modelName, out var modelType, out var viewModelType))
            {
                return NotFound(Errors.NullModel);
            }


            formData.Remove("modelType");

            ValidateRequiredFields(formData, viewModelType);


            // var user = Utils.GetCurrentUserName(User);
            var user = "Jabagh";


            // Create/update the instance of the model using the specified modelType
            var modelInstance = isCreateOperation
                ? _dataService.CreateModel(user, modelType, formData)
                : _dataService.UpdateModel(user, modelType, formData);

            // Return a success response with the created/updated model
            return Ok(new { success = true, message = $"{(isCreateOperation ? "Create" : "Update")} operation successful", data = modelInstance });
        }

        private void ValidateRequiredFields(Dictionary<string, object> formData, Type viewModelType)
        {
            var requiredProperties = viewModelType
                .GetProperties()
                .Where(prop => prop.GetCustomAttribute<RequiredAttribute>() != null)
                .Select(prop => prop.Name);

            foreach (var requiredProperty in requiredProperties)
            {
                if (!formData.ContainsKey(requiredProperty) || formData[requiredProperty]?.ToString() == "")
                {
                    throw new Exception($"Required field '{requiredProperty}' is missing or null.");
                }
            }
        }

        #endregion


    }
}
