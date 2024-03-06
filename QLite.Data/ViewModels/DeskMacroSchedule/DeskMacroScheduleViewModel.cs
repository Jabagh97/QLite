using QLite.Data.CustomAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLite.Data.ViewModels.DeskMacroSchedule
{
    public class DeskMacroScheduleViewModel
    {
        [Required]

        public string Name { get; set; }

        [Required]

        public string Account { get; set; }
        [Required]

        public int Desk { get; set; }

        [Boolean]

        public int HaftalikRutin { get; set; }

        [Boolean]

        public int Pasif { get; set; }

        [Boolean]

        public bool? D1 { get; set; }
        [Boolean]

        public bool? D2 { get; set; }
        [Boolean]

        public bool? D3 { get; set; }
        [Boolean]

        public bool? D4 { get; set; }
        [Boolean]

        public bool? D5 { get; set; }
        [Boolean]

        public bool? D6 { get; set; }
        [Boolean]

        public bool? D7 { get; set; }
    }
}
