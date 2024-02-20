using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QLite.Data.Models.Enums;

namespace QLite.Data.Dtos
{
    public class HwStatusDto
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public QLDevice Device { get; set; }
        public bool Ok { get; set; }
        public bool Connected { get; set; }
        public List<string> Status { get; set; }
    }
}
