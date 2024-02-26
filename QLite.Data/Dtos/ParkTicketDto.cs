using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLite.Data.Dtos
{
    public class ParkTicketDto 
    {
        public Guid TicketId { get; set; }
        public string TicketNote { get; set; }

        public Guid DeskID { get; set; }

    }
}
