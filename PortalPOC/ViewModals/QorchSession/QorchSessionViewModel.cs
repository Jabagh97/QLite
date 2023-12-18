namespace PortalPOC.ViewModals.QorchSession
{
    internal class QorchSessionViewModel
    {

        public string? Account { get; set; }

        public string? KioskApplication { get; set; }

        public string? Segment { get; set; }

        public string? ServiceType { get; set; }

        public bool? Success { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? StartTimeUtc { get; set; }

        public DateTime? EndTime { get; set; }

        public DateTime? EndTimeUtc { get; set; }

        public string? CurrentStep { get; set; }

        public string? InputValue { get; set; }

        public string? InputInfo { get; set; }

        public string? InputType { get; set; }

        public string? Workflow { get; set; }

        public string? Error { get; set; }

    }
}