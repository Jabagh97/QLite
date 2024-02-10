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

        //public string? Pano { get; set; }
        [Required]

        public string? DisplayNo { get; set; }

        public int? ActivityStatus { get; set; }

        public int? CurrentTicketNumber { get; set; }

        public DateTime? LastStateTime { get; set; }
        [Boolean]

        public bool? Autocall { get; set; }

        public string? ActiveUser { get; set; }
    }
}