using QLite.Data.CustomAttribute;
using System.ComponentModel.DataAnnotations;

namespace QLite.Data.ViewModels.Macro
{
    internal class MacroViewModel
    {

        public string? Account { get; set; }

        public int? MacroType { get; set; }
        [Required]

        public string? Name { get; set; }
        [Boolean]

        public int? ToThisDesk { get; set; }

    }
}