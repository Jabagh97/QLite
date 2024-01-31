using System;
using System.Collections.Generic;
using System.Text;

namespace Quavis.QorchLite.Data.Dto
{
    public class TransferTargetsDto
    {
        public List<DeskDto> TargetDesks { get; set; }
        public List<ServiceTypeDto> TargetServiceTypes { get; set; }
    }
}
