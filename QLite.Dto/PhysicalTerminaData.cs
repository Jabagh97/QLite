namespace QLite.Dto
{
    using QLite.Dto.Kapp;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using static QLite.Dto.Enums;

    public class PhysicalTerminaData
    {
        public int TerminalNo { get; set; }
        public DeskActivityStatus Status { get; set; }
        public JwtToken Token { get; set; }

    }
}
