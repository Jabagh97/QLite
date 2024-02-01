using System;
using System.Collections.Generic;
using System.Reflection;

namespace QLite.Data.Models.ViewModels
{
    public class TabPartialViewModel
    {
        public IEnumerable<PropertyInfo>? ICollectionProperties { get; set; }
        public Type ModelType { get; set; }
        public Dictionary<string, string>? FormData { get; set; }
    }
}
