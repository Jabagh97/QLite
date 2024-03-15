using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using QLite.Data.Services;
using QLiteDataApi.Constants;
using QLiteDataApi.Helpers;
using QLiteDataApi.Services;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace QLiteDataApi.Controllers.Admin
{
    public class AdminController : Controller
    {
        private readonly IModelTypeMappingService _modelTypeMappingService;
        private readonly IAdminService _dataService;

        public AdminController(IAdminService dataService, IModelTypeMappingService modelTypeMappingService)
        {
            _dataService = dataService;
            _modelTypeMappingService = modelTypeMappingService;
        }

        [HttpGet]
        [Route("api/Admin/GetData")]
        public IActionResult GetData(string modelName, string searchValue, string sortColumn, string sortColumnDirection, int skip, int pageSize)
        {
            try
            {
                if (!_modelTypeMappingService.TryGetModelTypes(modelName, out var modelType, out var viewModelType))
                {
                    return NotFound(Errors.NullModel);
                }

                var filteredData = _dataService.GetFilteredAndPaginatedData(modelType, viewModelType, searchValue, sortColumn, sortColumnDirection);
                //var paginatedData = filteredData.Skip(skip).Take(pageSize).ToDynamicList();
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



        [HttpPost]
        [Route("api/Admin/Create")]

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
        [Route("api/Admin/Edit")]

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

        [HttpGet]
        [Route("api/Admin/GetDropDowns")]

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


        [HttpPost]
        [Route("api/Admin/Delete")]

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

        [HttpGet]
        [Route("api/Admin/GetDbSetCollection")]
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

        [HttpPost]

        [Route("api/Admin/DeleteFromDbSetCollection")]

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
        [HttpGet]

        [Route("api/Admin/GetAllDesks")]

        public  IActionResult GetAllDesks()
        {
            var DeskList = _dataService.GetAllDesks();

            return Ok(DeskList);
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
