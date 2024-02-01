using System;
using System.Collections.Generic;
using System.Text;

namespace QLite.Dto
{
    public class UserSettingDto
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public bool SaveLocalStorage { get; set; }
    }
}
