using QLite.Data.CustomAttribute;
using System.ComponentModel.DataAnnotations;

namespace QLite.Data.ViewModels.KioskApplication
{
    internal class KioskApplicationViewModel
    {

        public string? Account { get; set; }

        public string? Branch { get; set; }
        [Required]

        public string? KioskApplicationType { get; set; }
        [Required]

        public string? KappWorkflow { get; set; }
        [Required]

        public string? KappName { get; set; }
        [Required]

        public string? HwId { get; set; }

        public string? DesignTag { get; set; }

        public string? PlatformAuthClientId { get; set; }

        public string? PlatformAuthClientSecret { get; set; }
        [Boolean]

        public bool? Active { get; set; }
        [Boolean]

        public bool? HasDigitalDisplay { get; set; }
    }
}