using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLite.Data.Dtos
{
    public class MacroRuleDto
    {
        public Guid Oid { get; set; }
        public int NumberOfTickets { get; set; }
        public int? Sequence { get; set; }

        public Guid? ServiceType { get; set; }

        public Guid? Segment { get; set; }
    }
}
