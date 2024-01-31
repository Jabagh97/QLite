using System.Reflection;

namespace PortalPOC.ViewModels
{
    public class TabPartialViewModel
    {
        public IEnumerable<PropertyInfo>? ICollectionProperties { get; set; }
        public Type ModelType { get; set; }
        public Dictionary<string, string>? FormData { get; set; }
    }
}
