using System;
using System.Collections.Generic;
using System.Text;
using static Quavis.QorchLite.Data.Dto.Enums;

namespace Quavis.QorchLite.Data.Dto
{
    public class DeskInitData
    {
        public TicketStateDto CurrentService { get; set; }

        public List<TicketStateDto> WaitingTickets { get; set; }

        public List<TicketStateDto> CompletedServices { get; set; }

        public List<DeskDto> AvailableDeskList { get; set; }

        public List<ServiceTypeDto> AvailableServiceTypes { get; set; }
        public List<MacroDto> MacroList { get; set; }

        public string DeskId{ get; set; }

        //public string DefaultSegmentId { get; set; }

        public DeskActivityStatus DeskStatus { get; set; }

        public bool Autocall { get; set; }

        public bool Disabled => DeskStatus != DeskActivityStatus.Open;

        public bool CallByTicket { get; set; }
    }
}
