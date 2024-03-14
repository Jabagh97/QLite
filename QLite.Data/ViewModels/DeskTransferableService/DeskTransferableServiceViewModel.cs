using System;
using System.ComponentModel.DataAnnotations;

namespace QLite.Data.ViewModels.DeskTransferableService
{
    public class DeskTransferableServiceViewModel
    {
        [Required]
        public string Account { get; set; }
        [Required]

        public string Branch { get; set; }
        [Required]

        public string Desk { get; set; }
        [Required]

        public string ServiceType { get; set; }
    }
}