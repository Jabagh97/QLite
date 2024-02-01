namespace QLite.Data.ViewModels.Ticket
{
    internal class TicketViewModel
    {

        public string? Branch { get; set; }

        public string? ServiceType { get; set; }

        public string? CurrentDesk { get; set; }

        public string? Segment { get; set; }

       // public string? ServiceTypeName { get; set; }

      //  public string? SegmentName { get; set; }

      //  public string? LangCode { get; set; }

        public int? CurrentState { get; set; }

        public int? LastOpr { get; set; }

        public string? LastOprTime { get; set; }

       // public string? ToServiceType { get; set; }

       // public string? ToDesk { get; set; }

       // public string? CurrentDesk { get; set; }

        public int? Number { get; set; }

        public int? DayOfYear { get; set; }

        public int? Year { get; set; }

        public string? TicketNote { get; set; }

        public string? CustomerInfo { get; set; }

        public string? CustomerNo { get; set; }

        public string? CardNo { get; set; }

        public string? NationalId { get; set; }

       // public string? TicketPool { get; set; }

       // public string? CreatedByDesk { get; set; }
    }
}