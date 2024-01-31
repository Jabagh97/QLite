namespace Quavis.QorchLite.Data.Dto
{
    using Quavis.QorchLite.Data.Dto.Kapp;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using static Quavis.QorchLite.Data.Dto.Enums;

    public class PhysicalTerminaData
    {
        public int TerminalNo { get; set; }
        public DeskActivityStatus Status { get; set; }
        public JwtToken Token { get; set; }

    }
}
