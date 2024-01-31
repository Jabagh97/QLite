using System.Collections.Generic;

namespace Quavis.QorchLite.Hwlib.Call
{
    public class VCPhysicalTerminalSettings
    {
        public string VendorId { get; set; }
        public string ProductId { get; set; }

        public List<int> Terminals {get; set;}
    }
}
