using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLite.DesignComponents
{
    public class DesCompJsonConverter : JsonCreationConverter<DesCompData>
    {
        protected override DesCompData Create(Type objectType, JObject jObject)
        {
            if (jObject == null) throw new ArgumentNullException("jObject");

            var sti = jObject["TypeInfo"]?.ToString() ?? jObject["typeInfo"]?.ToString();

            var t = Type.GetType(sti);
            return (DesCompData)Activator.CreateInstance(t);
        }
    }
}
