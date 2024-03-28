using QLite.Data.CustomAttribute;
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
        [Boolean]

        public bool? ResetOnRange { get; set; }



        //[Required]

        //public string MaxWaitingTicketCount { get; set; }

        [Required]

        public string RangeStart { get; set; }
        [Required]

        public string RangeEnd { get; set; }

        [Boolean]
        public string NotAvailable { get; set; }
        [Required]

        public string ServiceCode { get; set; }
        [Required]

        public string CopyNumber { get; set; }
        [DateAtrribute]
        public string? ServiceStartTime { get; set; }
        [DateAtrribute]
        public string? ServiceEndTime { get; set; }



    }
}