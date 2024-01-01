using Microsoft.AspNetCore.Mvc;
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

                // Check if the model type is valid
                if (modelTypeMapping.TryGetValue(modelName, out var typeTuple))
                {
                    Type modelType = typeTuple.Item1;
                    Type viewModelType = typeTuple.Item2;

                    // Extract request parameters using the injected extractor
                    var parameters = requestExtractor.ExtractParameters(Request.Form);

                  
                    // Get filtered and paginated data from DataService
                    var filteredData = _dataService.GetFilteredAndPaginatedData(modelType, viewModelType, parameters.SearchValue, parameters.SortColumn, parameters.SortColumnDirection, modelTypeMapping);

                    // Paginate the data
                    var paginatedData = filteredData.Skip(parameters.Skip).Take(parameters.PageSize).ToDynamicList();

                    // Get total records count
                    var recordsTotal = filteredData.Count();

                    // Prepare JSON response
                    var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = paginatedData };

                    return Ok(jsonData);
                }
                else
                {
                    return NotFound($"Model type '{modelName}' not found.");
                }
            }
            catch (Exception ex)
            {
                
                
                return StatusCode(500, ex.Message + "  Internal Server Error");
            }
        }



        public IActionResult AddPopup(string modelName, string action)
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

                ViewBag.DropDowns = namesDictionary;
                ViewBag.ViewModel = typeTuple.Item2;
                ViewBag.Action = action; // Pass action from the query string

                return PartialView("GenericPartial", modelType);
            }
            catch (Exception ex)
            {
                // Log the exception
                return NotFound($"Model type '{modelName}' not found.  " + ex.Message);
            }
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult Create([FromBody] Dictionary<string, object> formData)
        {
            try
            {
                // Validate formData and create a model instance
                if (!formData.TryGetValue("modelType", out var modelTypeValue) || modelTypeValue == null)
                {
                    return BadRequest("Model type not provided.");
                }

                if (!(modelTypeValue is string modelTypeName))
                {
                    return BadRequest("Invalid model type provided.");
                }

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

        public IActionResult EditPopup(string modelName, string action, Dictionary<string, string> Data)
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

                // Pass the received data to the view using ViewBag
                ViewBag.Data = Data;
                ViewBag.DropDowns = namesDictionary;
                ViewBag.ViewModel = typeTuple.Item2;
                ViewBag.Action = action; // Pass action from the query string

                return PartialView("GenericPartial", modelType);
            }
            catch (Exception ex)
            {
                // Log the exception
                return NotFound($"Model type '{modelName}' not found.  " + ex.Message);
            }
        }



    }

}
