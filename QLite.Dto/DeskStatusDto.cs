
using Quavis.QorchLite.Data.Dto.Kapp;
using System;
using static Quavis.QorchLite.Data.Dto.Enums;

namespace Quavis.QorchLite.Data.Dto
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