using Microsoft.EntityFrameworkCore;
using PortalPOC.Models;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace PortalPOC.Helpers
{
    public static class QueriesHelper
    {
        // Helper method to check if a property is a navigation property
        public static bool IsNavigationProperty(PropertyInfo property) =>
            property.PropertyType.IsClass && property.PropertyType != typeof(string);

        public static object ConvertToType(string? value, Type targetType)
        {
            if (TryConvertToSpecificType(value, targetType, out var convertedValue))
            {
                return convertedValue;
            }

            // If no specific conversion is available, use Convert.ChangeType
            return Convert.ChangeType(value, targetType);
        }

        private static bool TryConvertToSpecificType(string? value, Type targetType, out object? convertedValue)
        {
            convertedValue = null;

            if (targetType == typeof(int) || targetType == typeof(int?))
            {
                if (string.IsNullOrEmpty(value))
                    return true;

                convertedValue = targetType == typeof(int) ? (object)int.Parse(value) : (object)(int?)int.Parse(value);
                return true;
            }
            else if (targetType == typeof(decimal) || targetType == typeof(decimal?))
            {
                if (string.IsNullOrEmpty(value))
                    return true;

                convertedValue = targetType == typeof(decimal) ? (object)decimal.Parse(value) : (object)(decimal?)decimal.Parse(value);
                return true;
            }
            else if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
            {
                if (string.IsNullOrEmpty(value))
                    return true;

                convertedValue = targetType == typeof(DateTime) ? (object)DateTime.Parse(value) : (object)(DateTime?)DateTime.Parse(value);
                return true;
            }
            else if (targetType == typeof(Guid) || targetType == typeof(Guid?))
            {
                if (string.IsNullOrEmpty(value))
                    return true;

                convertedValue = targetType == typeof(Guid) ? (object)Guid.Parse(value) : (object)(Guid?)Guid.Parse(value);
                return true;
            }
            else if (targetType == typeof(byte[]))
            {
                convertedValue = Encoding.UTF8.GetBytes(value);
                return true;
            }
            else if (targetType == typeof(byte?[]))
            {
                if (string.IsNullOrEmpty(value))
                    return true;

                var byteArray = Encoding.UTF8.GetBytes(value);
                convertedValue = byteArray.Select(b => (byte?)b).ToArray();
                return true;
            }

            return false;
        }

        public static object SetStandardProperties(object modelInstance)
        {
            SetPropertyIfExist(modelInstance, "Oid", Guid.NewGuid());
            SetPropertyIfExist(modelInstance, "CreatedBy", "YourCreatedByValue"); // TODO
            SetPropertyIfExist(modelInstance, "ModifiedBy", "YourModifiedByValue"); // TODO
            SetPropertyIfExist(modelInstance, "CreatedDate", DateTime.Now);
            SetPropertyIfExist(modelInstance, "CreatedDateUtc", DateTime.UtcNow);
            SetPropertyIfExist(modelInstance, "ModifiedDate", DateTime.Now);
            SetPropertyIfExist(modelInstance, "ModifiedDateUtc", DateTime.UtcNow);

            return modelInstance;
        }

        private static void SetPropertyIfExist(object modelInstance, string propertyName, object value)
        {
            var property = modelInstance.GetType().GetProperty(propertyName);
            property?.SetValue(modelInstance, value);
        }
    }

}
