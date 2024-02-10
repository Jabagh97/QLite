using System.ComponentModel.DataAnnotations;

namespace QLite.Data.ViewModels.Design
{
    internal class DesignViewModel
    {
        [Required]

        public string? Name { get; set; }
        [Required]

        public string? DesignData { get; set; }

        public string? Account { get; set; }

        public string? KioskApplicationType { get; set; }

        public string? DesignTag { get; set; }
        [Required]

        public string? WfStep { get; set; }

    }
}