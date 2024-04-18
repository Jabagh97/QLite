using KioskApp.HardwareManager.Display.Settings;

namespace KioskApp.HardwareManager.Display.Protocols
{
    public record TicketInfo
    {
        public int DisplayNo { get; init; }
        public string TicketNumber { get; init; }
        public int TicketDisplayNo { get; init; }
        public DisplayArrowDirection Direction { get; set; }
        public VCTerminalSettings Settings { get; init; }
        public int SlotNumber { get; init; }
    }

}
