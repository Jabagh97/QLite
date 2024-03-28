using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLite.Data.Dtos
{
    public class TicketStateResponse
    {
        public int recordsFiltered { get; set; }
        public int recordsTotal { get; set; }
        public List<TicketStateData> data { get; set; }
    }


    public class TicketStateData
    {
        public string desk { get; set; }
        public string callType { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string note { get; set; }

    }
}
