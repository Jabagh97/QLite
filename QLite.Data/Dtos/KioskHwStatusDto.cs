using System.Collections.Generic;
using static QLite.Data.Models.Enums;

namespace QLite.Data.Dtos
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

  
}
