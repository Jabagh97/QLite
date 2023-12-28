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

        //public IActionResult AddPopup(string modelName)
        //{
        //    var modelTypeMapping = _modelTypeMappingService.GetModelTypeMapping();


        //    if (string.IsNullOrEmpty(modelName) || !modelTypeMapping.TryGetValue(modelName, out var typeTuple))
        //    {
        //        // Return a JSON response for better control
        //        return Json(new { success = false, errorMessage = "Invalid model name." });
        //    }

        //    Type modelType = typeTuple.Item1;

        //    // Create a dictionary to store lists of names associated with property names
        //    var namesDictionary = new Dictionary<string, List<dynamic>>();

        //    // Check for properties of type Guid?
        //    var guidProperties = modelType.GetProperties().Where(p => p.PropertyType == typeof(Guid?));

        //    foreach (var guidProperty in guidProperties)
        //    {
        //        if (string.IsNullOrEmpty(guidProperty.Name) || !modelTypeMapping.TryGetValue(guidProperty.Name, out var relatedTypeTuple))
        //        {
        //            return PartialView("Error");
        //        }

        //        // Query the related entities to get a list of "Name" where Gcrecord == null
        //        var relatedEntities = _dataService.GetTypedDbSet(relatedTypeTuple.Item1).Cast<dynamic>();
        //        var names = relatedEntities
        //                    .Where(e => e.Gcrecord == null)
        //                    .Select(e => e.GetType().GetProperty("Name") != null ? e.Name : e.KappName)
        //                    .ToList();


        //        // Add the list of names to the namesDictionary
        //        namesDictionary[guidProperty.Name] = names;
        //    }

        //    ViewBag.DropDowns = namesDictionary;
        //    ViewBag.ViewModel = typeTuple.Item2;
        //    ViewBag.Action = "Create";
        //    return PartialView("GenericPartial", typeTuple.Item1);
        //}


    }

}
