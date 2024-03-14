using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLite.Data.Dtos
{
    public class TransferTicketDto 
    {
        public Guid TicketId { get; set; }
       
        public Guid TransferServiceType { get; set; }
       
        public Guid TransferToDesk { get; set; }

        public Guid TransferFromDesk { get; set; }

        public string TicketNote { get; set; }
    }
}
