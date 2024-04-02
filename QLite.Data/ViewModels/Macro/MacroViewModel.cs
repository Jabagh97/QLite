using QLite.Data.CustomAttribute;
using System.Collections.Generic;
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

        //[Enum]

        //public ToDesk ToThisDesk { get; set; }

        [NotForTable]
        public virtual ICollection<object> DeskMacroSchedules { get; } = new List<object>();
        [NotForTable]

        public virtual ICollection<object> MacroRules { get; } = new List<object>();

    }
}