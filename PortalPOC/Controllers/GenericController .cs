using Microsoft.AspNetCore.Mvc;
using PortalPOC.Context;
using System.Data.Entity;

namespace PortalPOC.Controllers
{
    public class GenericController : Controller
    {
        private readonly AppDbContext _dbContext;

        public GenericController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index(string tableName)
        {
            // Dynamically get the DbSet for the specified table
            var dbSet = _dbContext.GetType().GetProperty(tableName)?.GetValue(_dbContext, null);

            // Use reflection to get data from the table
            var modelList = dbSet?.GetType().GetMethod("ToList")?.Invoke(dbSet, null);

            return View(modelList);
        }

       
    }
}
