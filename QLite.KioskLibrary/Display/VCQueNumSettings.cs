using System.Collections.Generic;

namespace Quavis.QorchLite.Hwlib.Display
{
    public class VCQueNumSettings
    {
        public string VendorId { get; set; }
        public string ProductId { get; set; }
        public int MessageIdleTime { get; set; }
        public int FlashingCount { get; set; }
        public int DimmingTime { get; set; }
        public string BreakMessage { get; set; }
        public DisplayType DisplayType { get; set; }
        public List<MainQueNum> MainQueNums { get; set; }
    }
}
