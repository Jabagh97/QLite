namespace PortalPOC.ViewModels.TicketPool
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


        public string? RangeStart { get; set; }
        public string? RangeEnd { get; set; }
        public string? NotAvailable { get; set; }

        public string? ServiceCode { get; set; }
        public string? CopyNumber { get; set; }

        public string? ServiceStartTime { get; set; }
        public string? ServiceEndTime { get; set; }



    }
}