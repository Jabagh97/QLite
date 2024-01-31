using System;
using System.Collections.Generic;
using System.Text;

namespace Quavis.QorchLite.Data.Dto.Kapp
{
    public class KappSettingsDto : KappBaseDto
    {
        public string Branch { get; set; }
        public string Parameter { get; set; }
        public string ParameterValue { get; set; }
    }
}
