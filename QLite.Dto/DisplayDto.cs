using System;
using System.Collections.Generic;
using System.Text;

namespace QLite.Dto
{

    public class DisplayDto
    {
        public string KioskId { get; set; }

        public string CurrentServiceNumber { get; set; }

        public string DeskId { get; set; }
        
        public string DeskName { get; set; }
        public string DeskUserName { get; set; }
        public string DisplayNo { get; set; }

        public byte[] Icon { get; set; }

        public string GetIcon()
        {
            if (Icon == null || Icon.Length == 0)
                return null;

            return Convert.ToBase64String(Icon);
        }

        public bool IsMainDisplay { get; set; }

        public List<DisplayDto> Children { get; set; }

    }
}
