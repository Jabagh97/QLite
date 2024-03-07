using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLite.Data.Dtos
{
    public class TicketRequestDto
    {
        public Guid ServiceTypeId { get; set; }
        public Guid SegmentId { get; set; }
        public Guid TicketPoolId { get; set; }
       // public Guid CustomerId { get; set; }
    }
}
