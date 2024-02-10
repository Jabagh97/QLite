using System.ComponentModel.DataAnnotations;

namespace QLite.Data.ViewModels.TicketPool
{
    internal class TicketPoolViewModel
    {
       

        public string? Account { get; set; }


        public string? Branch { get; set; }


        public string? TicketPoolProfile { get; set; }



        public string? ServiceType { get; set; }

        public string? Segment { get; set; }
        //public string? KioskApplication { get; set; }
        public string? MaxWaitingTicketCount { get; set; }

        [Required]

        public string? RangeStart { get; set; }
        [Required]

        public string? RangeEnd { get; set; }
        public string? NotAvailable { get; set; }
        [Required]

        public string? ServiceCode { get; set; }
        [Required]

        public string? CopyNumber { get; set; }

        public string? ServiceStartTime { get; set; }
        public string? ServiceEndTime { get; set; }



    }
}