using QLite.Data.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using static QLite.Data.Models.Enums;

namespace QLite.Data.ViewModels.Design
{
    internal class DesignViewModel
    {

        [Icon]
        public string DesignImage { get; set; }


        [Required]

        public string Name { get; set; }

        [Design]
        [NotForTable]
        public string? DesignData { get; set; }
        [Required]

        public string Account { get; set; }
        
        public string? DesignTag { get; set; }
        [Required]
        [Enum]

        public Step WfStep { get; set; }

    }
}