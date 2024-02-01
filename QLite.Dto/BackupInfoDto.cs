using System;
using System.Collections.Generic;
using System.Text;

namespace QLite.Dto
{
    public class BackupInfoDto
    {
        public string FileName { get; set; }
        public DateTime CreatedDate { get; set; }
        public long Size { get; set; }

    }
}
