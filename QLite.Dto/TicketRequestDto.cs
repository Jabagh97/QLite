namespace QLite.Dto
{
    
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class TicketRequestDto
    {        
        public string ServiceTypeId { get; set; }
        public string SegmentId { get; set; }
        public string TicketPoolId { get; set; }
        public string CustomerId { get; set; }
    }
}
