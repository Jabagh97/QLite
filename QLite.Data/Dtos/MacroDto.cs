using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QLite.Data.Models.Enums;

namespace QLite.Data.Dtos
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
