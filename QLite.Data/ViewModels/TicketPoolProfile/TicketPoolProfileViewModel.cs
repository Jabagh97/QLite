using System.ComponentModel.DataAnnotations;

namespace QLite.Data.ViewModels.TicketPoolProfile
{
    internal class TicketPoolProfileViewModel
    {
        public string? Account { get; set; }
        [Required]

        public string? Name { get; set; }
    }
}