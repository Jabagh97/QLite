using Microsoft.EntityFrameworkCore;
using PortalPOC.Models;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;

namespace PortalPOC.Helpers
{
    public static class QueriesHelper
    {


        // Helper method to check if a property is a navigation property
        public static bool IsNavigationProperty(PropertyInfo property) =>
       property.PropertyType.IsClass && property.PropertyType != typeof(string);


    }
}
