using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QLite.Data.Models.Enums;

namespace QLite.Data.Dtos
{
    public class TicketDto : KappBaseDto
    {
        public int Number { get; set; }
        public string Branch { get; set; }
        public string BranchName { get; set; }
        public string ServiceType { get; set; }
        public string Desk { get; set; }
        public string DeskName { get; set; }
        public string Segment { get; set; }
        public string ServiceTypeName { get; set; }
        public string SegmentName { get; set; }
        public string CurrentDesk { get; set; }
        public string Prefix { get; set; }
        public int NumberOfWaitingTickets { get; set; }

        public TicketStateEnum CurrentState { get; set; }
        public TicketOprEnum? LastOpr { get; set; }
        public DateTime? LastOprTime { get; set; }
        public string ToDesk { get; set; }
        public string ToServiceType { get; set; }
        public string TicketNote { get; set; }
        public int GroupCount { get; set; }
        public string CustomerId { get; set; }
        public string NationalId { get; set; }
        public string CustomerNo { get; set; }
        public string CardNo { get; set; }
        public string CreatedByDesk { get; set; }

        public string ImageBase64 { get; set; }
        public string TicketFooterMsg { get; set; }
    }

}
