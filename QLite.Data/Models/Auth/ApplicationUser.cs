using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLite.Data.Models.Auth
{
    public class ApplicationUser : IdentityUser
    {


        public AccountType AccountType { get; set; }

        public bool IsActive { get; set; } = true;
        public string TwoFactorSecret { get; set; }
        public bool TwoFactorConfirmed { get; set; } = false;
        public bool QRless { get; set; } = false;

        public Guid Desk { get; set; }

        
      
    }

    public enum AccountType
    {
        DeskApp,     // 0
        Adminpotal,    // 1
    }
}
