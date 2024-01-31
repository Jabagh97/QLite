
using Quavis.QorchLite.Data.Dto.Kapp;
using System;
using System.Collections.Generic;
using System.Text;
using static Quavis.QorchLite.Data.Dto.Enums;

namespace Quavis.QorchLite.Data.Dto
{
    public class KappRoleDto : KappBaseDto
    {
        public string Name { get; set; }
        public QorchBusinessRole BusinessRole { get; set; }
    }
}
