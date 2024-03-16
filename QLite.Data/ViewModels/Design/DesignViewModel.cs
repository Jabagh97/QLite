using QLite.Data.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using static QLite.Data.Models.Enums;

namespace QLite.Data.ViewModels.Design
{
    internal class DesignViewModel
    {
        [Required]

        public string Name { get; set; }
        [Design]
        public string? DesignData { get; set; }
        [Required]

        public string Account { get; set; }
        
        public string? DesignTag { get; set; }
        [Required]
        [Enum]

        public WfStep WfStep { get; set; }

    }
}