﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Quavis.QorchLite.Data.Dto.Kapp
{
    public class JwtToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public DateTime expires_in { get; set; }
    }

}
