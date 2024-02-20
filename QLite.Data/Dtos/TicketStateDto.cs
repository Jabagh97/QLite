using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QLite.Data.Models.Enums;

namespace QLite.Data.Dtos
{
    public class TicketStateDto : KappBaseDto
    {
        public string Branch { get; set; }

        public string Ticket { get; set; }

        public string Desk { get; set; }

        public string User { get; set; }

        public string DeskName { get; set; }

        public string ServiceType { get; set; }

        public string Macro { get; set; }

        public string DisplayNo { get; set; }

        public int TicketNumber { get; set; }

        public TicketDto TicketObj { get; set; }

        public MacroDto MacroObj { get; set; }

        public string Prefix { get; set; }

        public TicketStateEnum TicketStateValue { get; set; }

        public TicketOprEnum? TicketOprValue { get; set; }

        public string Segment { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? StartTimeLocal { get; set; }

        public DateTime? EndTime { get; set; }

        public DateTime? EndTimeLocal { get; set; }

        public string ServiceTypeName { get; set; }

        public string SegmentName { get; set; }

        public string CallingRuleDescription { get; set; }

        public string Duration { get; set; }

        public string TotalWaitDuration { get; set; }

        public string TotalServiceDuration { get; set; }

        public string TotalDuration { get; set; }

        public DeskAppType DeskAppType { get; set; }

        public TicketCallType? TicketCallType { get; set; }

        //[Ignore]
        //public string EndTimeDisplay
        //{
        //    get
        //    {
        //        return EndTime == null ? "-" : EndTime.Value.TimeOfDay.ToString();
        //    }
        //}

    }

}
