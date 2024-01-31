using System.Reflection;

namespace PortalPOC.ViewModels
{
    public class FormPartialViewModel
    {
        public string Action { get; set; }
        public IEnumerable<PropertyInfo> FilteredProperties { get; set; }
        public IDictionary<string, List<dynamic>> DropDowns { get; set; }
        public Dictionary<string, string> FormData { get; set; }
        public IEnumerable<PropertyInfo> ViewModelProperties { get; set; }
    }
}
