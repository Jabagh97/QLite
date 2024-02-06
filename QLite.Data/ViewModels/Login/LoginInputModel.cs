using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QLite.Data.ViewModels.Login
{
    public class LoginInputModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberLogin { get; set; }
        public string ReturnUrl { get; set; }

        [DisplayName("OTP Code")]
        public string OTP { get; set; }
    }
}
