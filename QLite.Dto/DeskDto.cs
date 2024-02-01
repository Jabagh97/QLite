
using QLite.Dto.Kapp;
using System;
using System.Collections.Generic;
using System.Text;
using static QLite.Dto.Enums;

namespace QLite.Dto
{
    public class DeskDto : KappBaseDto
    {
        public string Name { get; set; }

        public string Account { get; set; }

        public string Branch { get; set; }

        public string MacId { get; set; }
        public string Pano { get; set; }

        public string DisplayNo { get; set; }

        public DeskActivityStatus ActivityStatus { get; set; }

        public int? CurrentTicketNumber { get; set; }

        public DateTime? LastStateTime { get; set; }

        public string ActiveUser { get; set; }

        public string BranchName { get; set; }

        public bool? Autocall { get; set; }
    }
}
