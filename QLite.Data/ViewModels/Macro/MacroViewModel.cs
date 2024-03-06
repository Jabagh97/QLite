using QLite.Data.CustomAttribute;
using System.ComponentModel.DataAnnotations;
using static QLite.Data.Models.Enums;

namespace QLite.Data.ViewModels.Macro
{
    internal class MacroViewModel
    {

        [Required]

        public string Name { get; set; }

        [Required]

        public string Account { get; set; }
        [Enum]

        public MacroType MacroType { get; set; }
      
        [Enum]

        public ToDesk ToThisDesk { get; set; }

    }
}