using System.Collections.Generic;

namespace Quavis.QorchLite.Data.Dto.Kapp
{
    public class KioskAppSettings : AppSettings
    {
        public string CAMEndpoint { get; set; }
        public MockSettings MockSettings { get; set; }
        public List<PhysicalTerminalSettings> PhysicalTerminals { get; set; }
    }

    public class MockSettings
    {
        public bool UseMock { get; set; }
        public string CompanyCode { get; set; }
        public string AppCompanyCode { get; set; }
        public string KioskHwId { get; set; }
        public string CompanyId { get; set; }
        public string AppId { get; set; }
    }

    public class PhysicalTerminalSettings
    {
        public int TerminalNo { get; set; }
    }
}
