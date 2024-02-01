using System;
using System.Collections.Generic;
using System.Text;

namespace QLite.Dto.Kapp
{
    public class KappInitialSettingsDto
    {

        public List<KappSettingsDto> KappSettings { set; get; }
        public List<LanguageDto> Languages { set; get; }


    }
}
