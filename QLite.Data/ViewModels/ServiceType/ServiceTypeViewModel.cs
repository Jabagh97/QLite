namespace QLite.Data.ViewModels.ServiceType
{
    internal class ServiceTypeViewModel
    {

        public string? Account { get; set; }

        public string? Key { get; set; }

        public string? Name { get; set; }

        public byte[]? Icon { get; set; }

        public string? Parent { get; set; }

        public bool? IsParent { get; set; }

        public bool? CallInKiosk { get; set; }

        public bool? GenTicketByDesk { get; set; }

        public bool? Default { get; set; }

        public int? SeqNo { get; set; }
    }
}