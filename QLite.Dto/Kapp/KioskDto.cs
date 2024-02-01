using System;
using System.Collections.Generic;
using static QLite.Dto.Enums;

namespace QLite.Dto.Kapp
{
    public class KioskDto : KappBaseDto
    {

        public string HwId { get; set; }
        public string Branch { get; set; }
        public bool HasDigitalDisplay { get; set; }

        //Sadece Dto da var monitor için gerekli bilgiyi taşımak için
        //public string SessionStep { get; set; }
        //public KioskActivityStatus? ActivityStatus { get; set; } = null;
        //public WebSocketClientType? ClientType { get; set; } = null;

        
        public string Name { get; set; }
        public bool Active { get; set; }
        public string DisplayNo { get; set; }
        public KioskType KioskType { get; set; }
    }
}
