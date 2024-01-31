using System;
using System.Collections.Generic;
using System.Text;

namespace Quavis.QorchLite.Data.Dto
{

    public class DisplayTicketDto
    {
        public string KioskId { get; set; }

        public string QNumber { get; set; }

        public string ServiceName { get; set; }

        public string DeskName { get; set; }
        //public string DeskUserName { get; set; }

        public string DisplayNo { get; set; }

        public string DeskId { get; set; }

        public bool SendToMain { get; set; }
    }
}
