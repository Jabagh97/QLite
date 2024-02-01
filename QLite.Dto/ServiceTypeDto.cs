namespace QLite.Dto
{
    using QLite.Dto.Kapp;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class ServiceTypeDto : KappBaseDto
    {
        public string Account { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public bool IsParent { get; set; }
        public string Parent { get; set; }
        public byte[] Icon { get; set; }
        public bool CallInKiosk { get; set; }
        public int SeqNo { get; set; }
        public string TicketPool { get; set; }
        public DateTime ServiceStartTime { get; set; }
        public DateTime ServiceEndTime { get; set; }
        public DateTime BreakStartTime { get; set; }
        public DateTime BreakEndTime { get; set; }
        public string FontColor { get; set; }
        public string BtnColor { get; set; }
    }
}
    