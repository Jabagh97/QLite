
using QLite.Dto.Kapp;
using System;
using static QLite.Dto.Enums;

namespace QLite.Dto
{
    public class DeskStatusDto : KappBaseDto
    {
        public string Desk { get; set; }
        public string User { get; set; }
        public DeskActivityStatus DeskActivityStatus { get; set; }
        public DateTime? StateEndTime { get; set; }
        public DateTime? StateStartTime { get; set; }

    }
}