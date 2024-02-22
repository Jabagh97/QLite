using QLite.Data.CustomAttribute;
using System.ComponentModel.DataAnnotations;

namespace QLite.Data.ViewModels.ServiceType
{
    internal class ServiceTypeViewModel
    {

        public string? Account { get; set; }

        [Required]

        public string Name { get; set; }
     

        public string Parent { get; set; }
        [Boolean]

        public bool? IsParent { get; set; }
        [Boolean]

        public bool? CallInKiosk { get; set; }
        [Boolean]

        public bool? GenTicketByDesk { get; set; }
        [Boolean]

        public bool? Default { get; set; }

        public int? SeqNo { get; set; }
    }
}