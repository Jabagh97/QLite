using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLite.Data.ViewModels.Account
{
    public class AccountSetupModel
    {
        [DisplayName("New Password")]
        public string NewPassword { get; set; }
        [DisplayName("Confirm New Password")]
        public string ConfirmNewPassword { get; set; }
        [DisplayName("OTP Code")]
        public string OTP { get; set; }
        public string EncodedQR { get; set; }

        public string Email { get; set; }

        public string PasswordChangeToken { get; set; } // hidden
        public string UserId { get; set; } // hidden

        public bool QRless { get; set; } = false;
    }
}
