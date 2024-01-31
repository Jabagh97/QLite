using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quavis.QorchLite.Data.Dto
{

    public class KioskHwStatusDto
    {
        public List<HwStatusDto> HwStatusList { get; set; }
        public bool Ok { get; set; }

        public void AddHwStatus(HwStatusDto hwsdto)
        {
            if (HwStatusList == null)
                HwStatusList = new List<HwStatusDto>();
            HwStatusList.Add(hwsdto);
        }

        public static KioskHwStatusDto DeviceConnStatus(QLDevice dev, bool connected)
        {
            var r = new KioskHwStatusDto();
            r.HwStatusList = new List<HwStatusDto>();
            r.HwStatusList.Add(new HwStatusDto { Device = dev, Connected = connected, Ok = connected });
            r.Ok = connected;
            return r;
        }
    }

    public class HwStatusDto
    {
        [JsonConverter(typeof(StringEnumConverter))] 
        public QLDevice Device {get; set;}
        public bool Ok { get; set; }
        public bool Connected {get; set; }
        public List<string> Status  {get; set; }
    }
}
