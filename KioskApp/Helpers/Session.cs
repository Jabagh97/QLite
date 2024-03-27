using QLite.DesignComponents;

namespace KioskApp.Helpers
{
    public class Session
    {
        public static HomeAndDesPageDataViewModel homeAndDesPageData { get; set; }

        public static TicketAndDesPageDataViewModel ticketAndDesPageData { get; set; }

        public static SegmentsAndDesignModel segmentsAndDesignModel { get; set; }


        public static ServicesAndDesignModel servicesAndDesignModel { get; set; }


        public static Guid selectedSegment { get; set; }

    }
}
