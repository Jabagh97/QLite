using Microsoft.AspNetCore.Mvc;
using PortalPOC.Helpers;
using PortalPOC.Services;
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



        public IActionResult AddPopup(string modelName)
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
                ViewBag.Action = "Create";

                return PartialView("GenericPartial", modelType);
            }
            catch (Exception ex)
            {
             
                return NotFound($"Model type '{modelName}' not found.");
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] Dictionary<string, object> formData)
        {
            try
            {
               

                // Return a success response
                return Ok(new { success = true, message = "Model created successfully" });
            }
            catch (Exception ex)
            {
                // Handle exceptions and return an error response
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }


    }

}
