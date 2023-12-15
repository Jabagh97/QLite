using Microsoft.AspNetCore.Mvc;
using PortalPOC.Context;
using PortalPOC.Models;
using System.Data.Entity;

namespace PortalPOC.Controllers
{
    public class GenericTableController : Controller
    {
        private readonly QuavisQorchAdminEasyTestContext _dbContext;

        // Define a dictionary to map model names to types
        private readonly Dictionary<string, Type> modelTypeMapping = new Dictionary<string, Type>
    {
        {"Branch", typeof(Branch)}, 
    };

        public GenericTableController(QuavisQorchAdminEasyTestContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index(string modelName)
        {
            if (!modelTypeMapping.TryGetValue(modelName, out Type modelType))
            {
                // Handle the case when the modelName is not found in the mapping
                return NotFound();
            }

            return View(modelType);
        }
    }

}
