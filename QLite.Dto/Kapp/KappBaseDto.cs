using System;
using System.Collections.Generic;
using System.Text;

namespace Quavis.QorchLite.Data.Dto.Kapp
{   
    public class KappBaseDto
    {
        public string Oid { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? ModifiedDateUtc { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

    }
}
