using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PortalPOC.ViewModels.Branch
{
    public class BranchCreateEditModel
    {

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Branch Code")]
        public string BranchCode { get; set; }

        [Display(Name = "Terminal Name")]
        public string? Terminal { get; set; }

        public string? Area { get; set; }

        [Display(Name = "Adres 1")]
        public string? Address { get; set; }

        [Display(Name = "Adres 2")]
        public string? Address2 { get; set; }

        //DROPDOWNS
        public List<SelectListItem>? CountryList { get; set; }
       
        public Guid? Country { get; set; }

        public List<SelectListItem>? ProvinceList { get; set; }
       
        public Guid? Province { get; set; }

        public List<SelectListItem>? SubProvinceList { get; set; }

      
        [Display(Name = "Sub Province")]
        public Guid? SubProvince { get; set; }

        public List<SelectListItem>? KioskRestartProfileList { get; set; }

        [Display(Name = "Kiosk Restart Profile")]
        public Guid? KioskRestartProfile { get; set; }

        public List<SelectListItem>? AccountList { get; set; }
       
        public Guid? Account { get; set; }
    }
}
