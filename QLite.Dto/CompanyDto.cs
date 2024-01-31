using Quavis.QorchLite.Data.Dto.Kapp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quavis.QorchLite.Data.Dto
{
    public class CompanyDto : KappBaseDto
    {
        public string Name { get; set; }
        public byte[] Logo { get; set; }
        public string TicketFooterMsg { get; set; }
        public byte[] TicketLogo { get; set; }
    }
}
