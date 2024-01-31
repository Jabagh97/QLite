namespace Quavis.QorchLite.Hwlib.Call
{
    using System;

    public class InterfacePackage
    {
        public string PackageContent { get; set; }
        public DateTime PackageTime { get; set; }
        public PhysicalTerminalAction PTerminalAction { get; set; }
        public string TicketNo { get; set; }
        public int TerminalNo { get; set; }
        public DeviceType ReceivedDeviceType { get; set; }
        //public int PrinterNo { get; set; }
        //public bool PaperRollIsEmpty { get; set; }
    }

    public enum DeviceType
    {
        PhysicalTerminal = 1,
        PushButtonPrinter = 2,
        DiagnosticCapableDisplay = 3
    }

    public enum PhysicalTerminalAction
    {
        NewCallButton = 1,
        WaitingButton = 2,
        SleepModeButton = 3,
        ProgramButton = 4,
        ServiceTransferButton = 5,
        TerminalTransferButton = 6,
        TerminalExist = 7,
        LoginRequest = 8,
        ApproximateTicketCount = 9,
        WaitingTimeOfLastCall = 10,
        LoginNeeded = 11
    }
}
