using QLite.Data.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using static QLite.Data.Models.Enums;

namespace QLite.Data.ViewModels.Kiosk
{
    internal class KioskViewModel
    {

        public string? Account { get; set; }

        public string? Branch { get; set; }
        [Required]
        [Enum]
        public KioskType KioskType { get; set; }
       

       
        [Required]

        public string? Name { get; set; }
        [Required]

        public string? HwId { get; set; }
        [Required]
        [Enum]
        public WorkFlowType WorkFlowType { get; set; }

        [Required]

        [Boolean]

        public bool? Active { get; set; }
      
    }
}