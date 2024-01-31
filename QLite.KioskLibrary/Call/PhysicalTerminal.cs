using System;
using static Quavis.QorchLite.Data.Dto.Enums;

namespace Quavis.QorchLite.Hwlib.Call
{
    public class PhysicalTerminal
    {
        public int TerminalNo { get; set; }
        public int CommunicationLineNo { get; set; }
        public bool TerminalIsActive { get; set; }
        public bool CustomerIsAvailable { get; set; }
        public bool TerminalIsLoggedIn { get; set; }
        public string EmployeeBadgeNo { get; set; }
    }

    public class QLTerminalModel
    {
        public string Token { get; set; }
        public DeskActivityStatus Status { get; set; }

        public DateTime TokenExpTime { get; set; }

        public string LastTicketNumber { get; set; }
    }

}
