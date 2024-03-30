using QLiteDataApi.Context;
using Serilog;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace QLiteDataApi.Helpers
{
    public static class QueriesHelper
    {

        public static string ConvertAndSaveImage(string base64String, string fileName, string suffix = "")
        {
            if (!string.IsNullOrWhiteSpace(base64String) && base64String.StartsWith(ApiContext.Config.GetValue<string>("SiteDomain")))
            {
                return base64String; // Already a URL to an existing image.
            }

            if (string.IsNullOrWhiteSpace(base64String))
            {
                return base64String; // Empty, return as is.
            }

            try
            {
                var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DesignerImages");
                Directory.CreateDirectory(imagesPath);

                // Determine the file extension and decode the base64 string accordingly.
                string fileExtension;
                byte[] fileData;
                if (base64String.Contains("data:image/svg+xml;base64,"))
                {
                    fileExtension = "svg";
                    // SVGs are XML text, decode base64 to a string.
                    var base64Data = base64String.Split(',')[1];
                    fileData = Convert.FromBase64String(base64Data);
                }
                else
                {
                    fileExtension = "png"; // Assume PNG for simplicity, adjust as needed for other types.
                    var base64Data = base64String.Split(',')[1];
                    fileData = Convert.FromBase64String(base64Data);
                }

                var filePath = Path.Combine(imagesPath, $"{fileName}_{suffix}.{fileExtension}");

                if (!File.Exists(filePath))
                {
                    File.WriteAllBytes(filePath, fileData); // Works for both binary data and text (SVG).
                }

                var siteDomain = ApiContext.Config.GetValue<string>("SiteDomain").TrimEnd('/');
                return $"{siteDomain}/DesignerImages/{fileName}_{suffix}.{fileExtension}";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to convert and save image");
                return null;
            }
        }


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
                convertedValue = Convert.FromBase64String(value);
                return true;
            }
            else if (targetType == typeof(byte?[]))
            {
                if (string.IsNullOrEmpty(value))
                    return true;

                var byteArray = Convert.FromBase64String(value);
                convertedValue = byteArray.Select(b => (byte?)b).ToArray();
                return true;
            }
            else if (targetType == typeof(bool) || targetType == typeof(bool?)) // Add support for nullable boolean types
            {
                if (string.IsNullOrEmpty(value))
                {
                    convertedValue = targetType == typeof(bool) ? (object)false : (object)(bool?)null;
                    return true;
                }

                // Check if the value is "on" (common for checkboxes)
                if (value.Trim().Equals("on", StringComparison.OrdinalIgnoreCase))
                {
                    convertedValue = targetType == typeof(bool) ? (object)true : (object)(bool?)true;
                    return true;
                }

                if (bool.TryParse(value, out var boolValue))
                {
                    convertedValue = targetType == typeof(bool) ? (object)boolValue : (object)(bool?)boolValue;
                    return true;
                }

                return false;
            }


            return false;
        }

        public static object SetStandardProperties(object modelInstance, string user, string? operation = null)
        {
            if (operation != null && operation.Contains("Create"))
            {
                SetPropertyIfExist(modelInstance, "Oid", Guid.NewGuid());
                SetPropertyIfExist(modelInstance, "CreatedBy", user);
                SetPropertyIfExist(modelInstance, "CreatedDate", DateTime.Now);
                SetPropertyIfExist(modelInstance, "CreatedDateUtc", DateTime.UtcNow);
            }
            else
            {
                SetPropertyIfExist(modelInstance, "ModifiedBy", user);

                SetPropertyIfExist(modelInstance, "ModifiedDate", DateTime.Now);
                SetPropertyIfExist(modelInstance, "ModifiedDateUtc", DateTime.UtcNow);
            }


            return modelInstance;
        }

        private static void SetPropertyIfExist(object modelInstance, string propertyName, object value)
        {
            var property = modelInstance.GetType().GetProperty(propertyName);
            property?.SetValue(modelInstance, value);
        }

        public static IQueryable HandleUniqueCases(Type modelType, ref IQueryable query)
        {
            var modelTypeName = modelType.Name;

            // Check if modelType.Name is VComponent, Qrole, or Quser
            if (modelTypeName == "Qrole" || modelTypeName == "Quser")
            {
                return query;
            }
            else
            {
                // For other model types, apply the filter Gcrecord == null
                return query = query.Where("Gcrecord == null");
            }
        }

        public static string RemoveNavigationKeyword(string propertyName)
        {
            const string navigationKeyword = "Navigation";
            return propertyName.Replace(navigationKeyword, string.Empty);
        }

        public static string FormatTime(double timeInMinutes)
        {
            // Calculate hours and minutes from the total minutes
            int hours = (int)timeInMinutes / 60;
            int minutes = (int)timeInMinutes % 60;

            // Return the formatted string
            return $"{hours}h {minutes}m";
        }

    }

}
