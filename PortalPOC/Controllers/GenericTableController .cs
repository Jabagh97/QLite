using Microsoft.AspNetCore.Mvc;
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
        public IActionResult GetData(string modelName)
        {
            // Extract request parameters
            var formData = Request.Form;
            var pageSize = int.Parse(formData["length"]);
            var skip = int.Parse(formData["start"]);
            var searchValue = formData["search[value]"];
            var sortColumn = formData[string.Concat("columns[", formData["order[0][column]"], "][name]")];
            var sortColumnDirection = formData["order[0][dir]"];

            var modelTypeMapping = _modelTypeMappingService.GetModelTypeMapping();


            // Check if the model type is valid
            if (modelTypeMapping.TryGetValue(modelName, out var typeTuple))
            {
                Type modelType = typeTuple.Item1;
                Type viewModelType = typeTuple.Item2;

                // Use Type directly to invoke the Set method
                var dbSet = _dataService.GetTypedDbSet(modelType);

                // Query data from the DbSet

                var data = dbSet.Where("Gcrecord == null");




                // Get filtered and paginated data from DataService
                var filteredData = _dataService.GetFilteredAndPaginatedData(modelType, viewModelType, data, searchValue, sortColumn, sortColumnDirection, modelTypeMapping);



                // Paginate the data
                var paginatedData = filteredData.Skip(skip).Take(pageSize).ToDynamicList();

                // Get total records count
                var recordsTotal = filteredData.Count();

                // Prepare JSON response
                var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = paginatedData };

                return Ok(jsonData);
            }
            else
            {
                return View("Error");
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
