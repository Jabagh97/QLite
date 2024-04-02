using QLite.Data.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using static QLite.Data.Models.Enums;

namespace QLite.Data.ViewModels.MacroRule
{
    public class MacroRuleViewModel
    {
        [Required]

        public string? Macro { get; set; }

        public string? ServiceType { get; set; }

        public string? Segment { get; set; }
        //[Boolean]

        //public bool? Transfer { get; set; }
        //[Enum]

        //public ToDesk ToThisDesk { get; set; }


        [Required]

        public int? Sequence { get; set; }

       // public string? Description { get; set; }
    }
}
