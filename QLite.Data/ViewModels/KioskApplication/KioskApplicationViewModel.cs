namespace QLite.Data.ViewModels.KioskApplication
{
    internal class KioskApplicationViewModel
    {

        public string? Account { get; set; }

        public string? Branch { get; set; }

        public string? KioskApplicationType { get; set; }

        public string? KappWorkflow { get; set; }

        public string? KappName { get; set; }

        public string? HwId { get; set; }

        public string? DesignTag { get; set; }

        public string? PlatformAuthClientId { get; set; }

        public string? PlatformAuthClientSecret { get; set; }

        public bool? Active { get; set; }

        public bool? HasDigitalDisplay { get; set; }
    }
}