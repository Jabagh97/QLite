using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLite.Data.ViewModels.Auth
{
    public class ApplicationUserViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        [DisplayName("Email Confirmed")]
        public bool EmailConfirmed { get; set; }
        [Required]
        public string Company { get; set; }
        [DisplayName("Account Type")]
        public string AccountType { get; set; }
        [DisplayName("Is Active")]
        public bool IsActive { get; set; }
        public string Id { get; set; }
    }
}
