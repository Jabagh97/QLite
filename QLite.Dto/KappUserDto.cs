using QLite.Dto.Kapp;
using System;
using System.Collections.Generic;
using System.Text;
using static QLite.Dto.Enums;

namespace QLite.Dto
{
    public class KappUserDto : KappBaseDto
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Branch { get; set; }
        public string Desk { get; set; }
        public string DeskName { get; set; }
        public string Account { get; set; }
        public string DeskPanoId { get; set; }
        public string StoredPassword { get; set; }
        public string BranchName { get; set; }
        public List<KappRoleDto> KappRoles { get; set; }
        public bool CanChangeMacro{ get; set; }

    }
}
