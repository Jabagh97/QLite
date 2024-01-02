using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PortalPOC.Helpers;
using PortalPOC.Services;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;

namespace PortalPOC.Controllers
{
    public class GenericTableController : Controller
    {
        private readonly IModelTypeMappingService _modelTypeMappingService;
        private readonly IDataService _dataService;

        public GenericTableController(IDataService dataService, IModelTypeMappingService modelTypeMappingService)
        {
            _dataService = dataService;
            _modelTypeMappingService = modelTypeMappingService;
        }

        public IActionResult Index(string modelName)
        {
            var modelTypeMapping = _modelTypeMappingService.GetModelTypeMapping();

            if (string.IsNullOrEmpty(modelName) || !modelTypeMapping.TryGetValue(modelName, out var typeTuple))
            {
                return View("Error");
            }

            return View(typeTuple.Item2);
        }

        [HttpPost]
        public IActionResult GetData(string modelName, [FromServices] IDataTableRequestExtractor requestExtractor)
        {
            try
            {
                var modelTypeMapping = _modelTypeMappingService.GetModelTypeMapping();

                if (!modelTypeMapping.TryGetValue(modelName, out var typeTuple))
                {
                    return NotFound($"Model type '{modelName}' not found.");
                }

                Type modelType = typeTuple.Item1;
                Type viewModelType = typeTuple.Item2;

                var parameters = requestExtractor.ExtractParameters(Request.Form);

                var filteredData = _dataService.GetFilteredAndPaginatedData(modelType, viewModelType, parameters.SearchValue, parameters.SortColumn, parameters.SortColumnDirection, modelTypeMapping);

                var paginatedData = filteredData.Skip(parameters.Skip).Take(parameters.PageSize).ToDynamicList();

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
        //[ValidateAntiForgeryToken]
        public IActionResult Create([FromBody] Dictionary<string, object> formData)
        {
            try
            {
                // Validate formData and create a model instance
                if (!formData.ContainsKey("modelType"))
                {
                    return BadRequest("Model type not provided.");
                }

                // Extract modelType from formData
                string modelTypeName = formData["modelType"].ToString();

                var modelTypeMapping = _modelTypeMappingService.GetModelTypeMapping();

                if (!modelTypeMapping.TryGetValue(modelTypeName, out var typeTuple))
                {
                    return NotFound($"Model type '{modelTypeName}' not found.");
                }

                Type modelType = typeTuple.Item1;

                formData.Remove("modelType");

                // Create an instance of the model using the specified modelType
                var modelInstance = _dataService.CreateModel(modelType, formData);

                // Return a success response with the created model
                return Ok(new { success = true, message = "Model created successfully", data = modelInstance });
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an error response
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public IActionResult Edit([FromBody] Dictionary<string, object> formData)
        {
            try
            {
                // Validate formData and create a model instance
                if (!formData.ContainsKey("modelType"))
                {
                    return BadRequest("Model type not provided.");
                }

                // Extract modelType from formData
                string modelTypeName = formData["modelType"].ToString();

                var modelTypeMapping = _modelTypeMappingService.GetModelTypeMapping();

                if (!modelTypeMapping.TryGetValue(modelTypeName, out var typeTuple))
                {
                    return NotFound($"Model type '{modelTypeName}' not found.");
                }

                Type modelType = typeTuple.Item1;

                formData.Remove("modelType");

                // Update the instance of the model using the specified modelType
                var modelInstance = _dataService.UpdateModel(modelType, formData);

                // Return a success response with the created model
                return Ok(new { success = true, message = "Model created successfully", data = modelInstance });
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an error response
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
                    return BadRequest("Model type not provided.");
                }

                // Extract modelType from formData
                string modelTypeName = Data["modelType"].ToString();

                var modelTypeMapping = _modelTypeMappingService.GetModelTypeMapping();

                if (!modelTypeMapping.TryGetValue(modelTypeName, out var typeTuple))
                {
                    return NotFound($"Model type '{modelTypeName}' not found.");
                }

                Type modelType = typeTuple.Item1;

                Data.Remove("modelType");

                // Create an instance of the model using the specified modelType
                var modelInstance = _dataService.SoftDelete(modelType, Data);

                // Return a success response with the created model
                return Ok(new { success = true, message = "Model created successfully", data = modelInstance });
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

                if (!modelTypeMapping.TryGetValue(modelName, out var typeTuple))
                {
                    return NotFound($"Model type '{modelName}' not found.");
                }

                Type modelType = typeTuple.Item1;

                var namesDictionary = _dataService.GetGuidPropertyNames(modelType, modelTypeMapping);

                if (!opType.IsNullOrEmpty() && opType.Contains("Edit"))
                {
                    ViewBag.Data = data;
                }

                ViewBag.DropDowns = namesDictionary;
                ViewBag.ViewModel = typeTuple.Item2;
                ViewBag.Action = opType;

                return PartialView("GenericPartial", modelType);
            }
            catch (Exception ex)
            {
                // Log the exception
                return NotFound($"Model type '{modelName}' not found. {ex.Message}");
            }
        }
    }

}
