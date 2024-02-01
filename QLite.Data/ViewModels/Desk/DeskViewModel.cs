using System;

namespace QLite.Data.ViewModels.Desk
{
    internal class DeskViewModel
    {   

        public string? Name { get; set; }

        public string? Account { get; set; }

        public string? Branch { get; set; }

        //public string? Pano { get; set; }

        public string? DisplayNo { get; set; }

        public int? ActivityStatus { get; set; }

        public int? CurrentTicketNumber { get; set; }

        public DateTime? LastStateTime { get; set; }

        public bool? Autocall { get; set; }

        public string? ActiveUser { get; set; }
    }
}