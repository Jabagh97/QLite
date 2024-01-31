using System.Collections.Generic;

namespace Quavis.QorchLite.Hwlib.Display
{
    public enum DisplayType
    {
        DotMatrix = 1,
        SevenSegment = 2,
        Lcd = 3,
        SevenSegment4Digit = 4
    }

    public class MainQueNum
    {
        public string Name { get; set; }
        public bool IsDedicated { get; set; }
        public List<DisplayRow> DisplayRows { get; set; }
        public int? RowCount => DisplayRows?.Count;
        public List<QueNum> QueNums { get; set; }
        public int DotWidth { get; set; }
        public int DotHeight { get; set; }
    }

    public class DisplayRow
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class QueNum
    {
        public string MainQueNumId { get; set; }
        public DisplayRow DisplayRow { get; set; }
        public int Direction { get; set; }
        public int SelfDirection { get; set; }
        public int DotWidth { get; set; }
        public int DotHeight { get; set; }
    }

    public class QueNumData
    {
        public string DisplayNo { set; get; }
        public string TicketNo { set; get; }
        public bool SendToMain { set; get; }
    }
}
