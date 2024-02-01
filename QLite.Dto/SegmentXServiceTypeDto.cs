
using QLite.Dto.Kapp;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLite.Dto
{
    public class SegmentXServiceTypeDto : KappBaseDto
    {
        public List<SegmentDto> SegmentDtos { get; set; }
        public List<ServiceTypeDto> ServiceTypeDtos { get; set; }
        public List<TicketDto> TicketDtos { get; set; }
    }
}
