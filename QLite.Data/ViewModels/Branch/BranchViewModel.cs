using System.ComponentModel.DataAnnotations;

namespace QLite.Data.ViewModels.Branch
{
    public class BranchViewModel
    {
        [Required]
        public string? Name { get; set; }

        public string? Account { get; set; }

        
        public string? Country { get; set; }

      
        public string? Province { get; set; }

     
      
        public string? SubProvince { get; set; }

        public string? BranchCode { get; set; }
        public string? Terminal { get; set; }
        public string? Area { get; set; }

   
       // public string? KioskRestartProfile { get; set; }
        public string? Address { get; set; }
        public string? Address2 { get; set; }

        public string? TicketPoolProfile { get; set; }

    }
}
