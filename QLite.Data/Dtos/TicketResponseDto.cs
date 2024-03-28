using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLite.Data.Dtos
{
    public class TicketResponse
    {
        public int recordsFiltered { get; set; }
        public int recordsTotal { get; set; }
        public List<TicketData> data { get; set; }
    }

    public class TicketData
    {
        public string ticketNumber { get; set; }
        public string service { get; set; }
        public string segment { get; set; }
        public string oid { get; set; }
    }
}
