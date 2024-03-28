using QLite.Data.CustomAttribute;
using System;
using System.ComponentModel.DataAnnotations;

namespace QLite.Data.ViewModels.Desk
{
    internal class DeskViewModel
    {
        [Required]

        public string? Name { get; set; }

        public string? Account { get; set; }

        public string? Branch { get; set; }

        public string? Kiosk { get; set; }


        [Required]

        public string? DisplayNo { get; set; }
        [NonEditable]
        public int? ActivityStatus { get; set; }
        [NonEditable]

        public int? CurrentTicketNumber { get; set; }
        [NonEditable]

        public DateTime? LastStateTime { get; set; }
        [Boolean]

        public bool? Autocall { get; set; }
        [NonEditable]

        public string? ActiveUser { get; set; }
    }
}