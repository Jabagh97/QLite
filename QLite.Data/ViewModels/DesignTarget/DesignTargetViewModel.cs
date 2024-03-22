using System;
using System.ComponentModel.DataAnnotations;

namespace QLite.Data.ViewModels.DesignTarget
{
    internal class DesignTargetViewModel
    {
        [Required]

        public string? Design { get; set; }
        [Required]

        public string? Account { get; set; }
        [Required]

        public string? Branch { get; set; }
        [Required]

        public string? Kiosk { get; set; }
    }
}