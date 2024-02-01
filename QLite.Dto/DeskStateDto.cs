
using QLite.Dto.Kapp;
using System;
using System.Collections.Generic;
using System.Text;
using static QLite.Dto.Enums;

namespace QLite.Dto
{
    public class DeskStateDto : KappBaseDto
    {
        public string BranchId { get; set; }
        public string DeskId { get; set; }
        public string UserId { get; set; }
        public DeskActivityStatus Status { get; set; }
        public string UserName { get; set; }
        public int? TicketNumber { get; set; }
    }
}
