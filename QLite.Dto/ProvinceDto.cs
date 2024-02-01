
using QLite.Dto.Kapp;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLite.Dto
{
    public class ProvinceDto : KappBaseDto
    {
        public string Country { get; set; }
        public string Name { get; set; }
        public int OptimisticLockField { get; set; }
    }
}
