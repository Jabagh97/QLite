using QLite.Data;

namespace KioskApp.Models
{
    public class KioskIndexModel
    {
        public string? KioskId { get; set; }

        public List<ServiceType>? ServiceTypeList { get; set; }
        public string? LogoData { get; set; }

        public static List<Resource>? Resources { get; set; }
        public string? PageStartHeight { get; set; }

        public static string GetLanguage(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;
            var paramValue = Resources?.FirstOrDefault(c => c.Parameter?.ToLower().Trim() == key.ToLower().Trim());
            if (paramValue != null)
                return paramValue.ParameterValue;
            return key;
        }
    }
}