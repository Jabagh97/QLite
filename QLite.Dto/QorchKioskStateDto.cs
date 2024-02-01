using System;
using System.Collections.Generic;
using System.Text;

namespace QLite.Dto
{
    public class QorchKioskStateDto
    {
        public string LogMsg { get; set; }
        public string KioskAppId { get; set; }
        public string KioskHwId { get; set; }
        public string CussState { get; set; }
        public string SessionStep { get; set; }
        public string SessionId { get; set; }
        public string CustomerInfo { get; set; }
        public string BranchId { get; set; }
    }
}
