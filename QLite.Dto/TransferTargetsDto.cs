using System;
using System.Collections.Generic;
using System.Text;

namespace QLite.Dto
{
    public class TransferTargetsDto
    {
        public List<DeskDto> TargetDesks { get; set; }
        public List<ServiceTypeDto> TargetServiceTypes { get; set; }
    }
}
