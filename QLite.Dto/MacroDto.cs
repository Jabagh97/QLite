
using Quavis.QorchLite.Data.Dto.Kapp;
using System;
using System.Collections.Generic;
using System.Text;
using static Quavis.QorchLite.Data.Dto.Enums;

namespace Quavis.QorchLite.Data.Dto
{
    public class MacroDto : KappBaseDto
    {
        public string Name { get; set; }
        public MacroType MacroType { get; set; }
        public int MaxWaitingTime { get; set; }
        public MeNotothersAll ToThisDesk { get; set; }
        public string Account { get; set; }
    }
}
