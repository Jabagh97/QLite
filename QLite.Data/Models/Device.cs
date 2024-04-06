using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QLite.Data.Models.Enums;

namespace QLite.Data
{
    public class Device
    {

        public Guid Oid { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime CreatedDateUtc { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public DateTime? ModifiedDateUtc { get; set; }

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }


        public string Name { get; set; }

        public Guid Kiosk { get; set; }

        public QLDevice DeviceType { get; set; }

        public string Settings { get; set; }

        public int? Gcrecord { get; set; }

        public virtual ICollection<Kiosk> Kiosks { get; } = new List<Kiosk>();

    }
}
