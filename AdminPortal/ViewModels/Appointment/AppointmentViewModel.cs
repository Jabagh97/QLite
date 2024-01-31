namespace PortalPOC.ViewModels.Appointment
{
    internal class AppointmentViewModel
    {
        public string? NationalId { get; set; }

        public string? PhoneNumber { get; set; }

        public DateTime? AppointmentDate { get; set; }

        public DateTime? BookingDate { get; set; }

        public string? Branch { get; set; }

        public string? ServiceType { get; set; }

        public string? Segment { get; set; }

    }
}