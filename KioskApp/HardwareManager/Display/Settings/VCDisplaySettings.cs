using KioskApp.HardwareManager.Display.Protocols;
using System.Drawing;
using static QLite.Data.Models.Enums;

namespace KioskApp.HardwareManager.Display.Settings
{
    public class VCDisplaySettingsNetwork : VCDisplaySettings<List<NetworkSettings>>
    {
    }

    public class VCDisplaySettingsUSB : VCDisplaySettings<UsbDeviceSettings>
    {
    }

    public abstract class VCDisplaySettings<T> : VCBaseSettings
    {

        public T RealDeviceSettings { get; set; }
        public string DisplayType { get; set; }
        public int InitialMarqueeDelay { get; set; }
        public VCTerminalSettings DefaultSettings { get; set; }

        public List<VCMainDisplay> MainDisplays { get; set; }
        public List<VCTerminalDisplay> Terminals { get; set; }


    }

    public class VCMainDisplay
    {
        public string Name { get; set; }
        public List<VCMainDisplayRow> RowIds { get; set; }
        public List<VCTerminal> Terminals { get; set; }
        public VCTerminalSettings Settings { get; set; }
    }

    public class VCMainDisplayRow
    {
        public byte RowId { get; set; }
        public string BreakMessage { get; set; }
    }

    public class VCTerminal
    {
        public byte RowId { get; set; }
        public byte DisplayNo { get; set; }
        public int Direction { get; set; }

        internal virtual bool IsMain { get; set; }
    }

    public class VCTerminalDisplay : VCTerminal
    {
        public VCTerminalSettings Settings { get; set; }
    }

    public class VCTerminalSettings
    {
        public int? DotWidth { get; set; }
        public int? DotHeight { get; set; }
        public int? ArabicDelayTime { get; set; }

        public int? MessageIdleTime { get; set; }
        public int? FlashingCount { get; set; }
        public int? DimmingTime { get; set; }
        public string BreakMessage { get; set; }
        public string BreakMessageArabic { get; set; }
        public VCTerminalSettingsFont Font { get; set; }
    }

    public class VCTerminalSettingsFont
    {
        public string FontName { get; set; }
        public float FontSize { get; set; }
        public FontStyle FontStyle { get; set; }
        public bool BitmapStrech { get; set; }
        public int BitmapContrast { get; set; }
        public FontWeightType? FontWeight { get; set; }
    }

}
