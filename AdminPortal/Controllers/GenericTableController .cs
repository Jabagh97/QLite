using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PortalPOC.Constants;
using PortalPOC.Helpers;
using PortalPOC.Services;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace PortalPOC.Controllers
{
    public class GenericTableController : Controller
    {
        private readonly IModelTypeMappingService _modelTypeMappingService;
        private readonly IDataService _dataService;
        private readonly IDataTableRequestExtractor _requestExtractor;



        public GenericTableController(IDataService dataService, IModelTypeMappingService modelTypeMappingService, IDataTableRequestExtractor requestExtractor)
        {
            _dataService = dataService;
            _modelTypeMappingService = modelTypeMappingService;
            _requestExtractor = requestExtractor;
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
        public IActionResult GetData(string modelName)
        {
            try
            {
                if (!_modelTypeMappingService.TryGetModelTypes(modelName, out var modelType, out var viewModelType))
                {
                    return NotFound(Errors.NullModel);
                }

                var parameters = _requestExtractor.ExtractParameters(Request.Form);

                var filteredData = _dataService.GetFilteredAndPaginatedData(modelType, viewModelType, parameters.SearchValue, parameters.SortColumn, parameters.SortColumnDirection);

                var paginatedData = filteredData.Skip(parameters.Skip).Take(parameters.PageSize).ToDynamicList();

                // var paginatedData = filteredData.ToDynamicList();


                var recordsTotal = filteredData.Count();

                var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = paginatedData };

                return Ok(jsonData);
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
                return ProcessModelOperation(formData, isCreateOperation: true);
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
                return ProcessModelOperation(formData, isCreateOperation: false);
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


        public IActionResult ShowPopup(string modelName, string opType, Dictionary<string, string> data)
        {
            try
            {
                var modelTypeMapping = _modelTypeMappingService.GetModelTypeMapping();


                if (!_modelTypeMappingService.TryGetModelTypes(modelName, out var modelType, out var viewModelType))
                {
                    return NotFound(Errors.NullModel);
                }

                var namesDictionary = _dataService.GetGuidPropertyNames(modelType, modelTypeMapping);

                if (!opType.IsNullOrEmpty() && opType.Contains(Operations.Edit))
                {
                    ViewBag.Data = data;
                }

                ViewBag.DropDowns = namesDictionary;
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


            var user = Utils.GetCurrentUserName(User);


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
