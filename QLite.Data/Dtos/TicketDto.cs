using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLite.Data.Dtos
{
    public class TicketDto
    {
        public Guid Oid { get; set; }
        public string ServiceTypeName { get; set; }

        public string SegmentName { get; set; }
        public int? Number { get; set; }

        public string? Desk { get; set; }

        public string ServiceCode { get; set; }

        public int WaitingTickets { get; set; }

    }
}
