using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QLite.Data.Models.Enums;

namespace QLite.Data.Dtos
{
    public class KioskDto
    {
        public Guid Oid { get; set; }

        public string Name { get; set; }

        public int? KioskType { get; set; }


        public string HwId { get; set; }
        public Guid? Branch { get; set; }


        public int? WorkFlowType { get; set; }

    }
}
