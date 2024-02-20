using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QLite.Data.Models.Enums;

namespace QLite.Data.Dtos
{
    public class QLiteRestApiClient
    {

        public string AppId { get; set; }
        public string Name { get; set; }
        public string BranchId { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public KioskType KioskType { get; set; }
    }
}
