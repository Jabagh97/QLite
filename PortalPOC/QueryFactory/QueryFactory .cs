
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PortalPOC.Helpers;
using PortalPOC.Models;

using System.Linq.Dynamic.Core;
using System.Reflection;

namespace PortalPOC.QueryFactory
{
  
    public class QueryFactory : IQueryFactory
    {

        public IQueryable SelectAndJoinQuery(IQueryable? data, Type modelType, Type viewModelType, QuavisQorchAdminEasyTestContext _dbContext)
        {
            if (data == null)
            {
                return Enumerable.Empty<object>().AsQueryable();
            }

            var query = data;
            var modelProperties = modelType.GetProperties();
            var viewModelProperties = viewModelType.GetProperties();

            var selectProperties = modelProperties
                .Where(mp => viewModelProperties.Any(vp => vp.Name == mp.Name));

            var leftJoinProperties = modelProperties
                .Where(mp => viewModelProperties.Any(vp => mp.Name.Contains(vp.Name)) && QueriesHelper.IsNavigationProperty(mp));

            var combinedProperties = selectProperties
                .Select(sp => leftJoinProperties.FirstOrDefault(lp => lp.Name.Contains(sp.Name)) ?? sp);

            var selectExpressions = combinedProperties.Select(property =>
            {
                var propertyToJoin = property.Name switch
                {
                    "KioskApplicationNavigation" => "KappName",
                    _ => "Name"
                };

                var navigationProperty = property.Name.Contains("Navigation")
                    ? property.Name.Replace("Navigation", "")
                    : null;

                return !navigationProperty.IsNullOrEmpty()
                    ? $"{property.Name} != null ? {property.Name}.{propertyToJoin} : null as {navigationProperty}"
                    : $"{property.Name} as {property.Name}";
            });

            var selectProjection = string.Join(", ", selectExpressions);

            return query.Select($"new ({selectProjection})");

          
        }

        public void CreateInstance(QuavisQorchAdminEasyTestContext _dbContext,object modelInstance)
        {
            // Ensure the modelInstance is not null
            if (modelInstance == null)
            {
                throw new ArgumentNullException(nameof(modelInstance), "Model instance cannot be null.");
            }

            modelInstance = QueriesHelper.SetStandardProperties(modelInstance);

            // Add the model instance to the DbContext
            _dbContext.Add(modelInstance);

            // Save changes to the database
            _dbContext.SaveChanges();
        }


    }
}
