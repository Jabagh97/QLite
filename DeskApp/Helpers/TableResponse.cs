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
        public string ticketNumber { get; set; }
        public string service { get; set; }
        public string segment { get; set; }
        public string oid { get; set; }
    }


    public class TicketStateResponse
    {
        public int recordsFiltered { get; set; }
        public int recordsTotal { get; set; }
        public List<TicketStateData> data { get; set; }
    }


    public class TicketStateData
    {
        public string desk { get; set; }
        public string callType { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string note { get; set; }

    }
}
