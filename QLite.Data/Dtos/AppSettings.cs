using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLite.Data.Dtos
{
    public class AppSettings
    {
        public string Hub { get; set; }
        public string Log { get; set; }
        public string Auth { get; set; }
        public string LogLevel { get; set; }
    }
}
