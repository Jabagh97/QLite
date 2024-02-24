namespace DeskApp.Helpers
{
    // Define a model class to represent the structure of the JSON response
    public class TicketResponse
    {
        public int recordsFiltered { get; set; }
        public int recordsTotal { get; set; }
        public List<TicketData> data { get; set; }
    }

    public class TicketData
    {
        public int ticketNumber { get; set; }
        public string service { get; set; }
        public string segment { get; set; }
        public string oid { get; set; }
    }
}
