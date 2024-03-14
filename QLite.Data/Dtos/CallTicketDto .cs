using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLite.Data.Dtos
{
    public class CallTicketDto
    {
        public Guid DeskID { get; set; }
        public Guid TicketID { get; set; }
        public Guid User { get; set; }
        public Guid MacroID { get; set; }
    }

}
