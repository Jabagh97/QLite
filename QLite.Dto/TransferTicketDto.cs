
using System;
using System.Collections.Generic;
using System.Text;
using static Quavis.QorchLite.Data.Dto.Enums;

namespace Quavis.QorchLite.Data.Dto
{
    public class TransferTicketDto //: KappBaseDto
    {
        public string TicketId { get; set; }
        public int? TicketNumber { get; set; }
        public string TransferServiceType { get; set; }
        public int? ServiceNo { get; set; }
        public string TransferDesk { get; set; }
        public int? TransferDeskNo { get; set; }
        public string TransferServiceTypeName { get; set; }
        public string TicketNote { get; set; }
    }
}
