
using QLite.Dto.Kapp;
using System;
using System.Collections.Generic;
using System.Text;
using static QLite.Dto.Enums;

namespace QLite.Dto
{
    public class KappRoleDto : KappBaseDto
    {
        public string Name { get; set; }
        public QorchBusinessRole BusinessRole { get; set; }
    }
}
