using System;
using System.Collections.Generic;
using System.Text;
using static QLite.Dto.Enums;

namespace QLite.Dto
{
    public class UserLoginDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string BranchId { get; set; }
        public string DeskId { get; set; }
        public string MacId { get; set; }
        public DeskAppType DeskAppType { get; set; }


    }
}
