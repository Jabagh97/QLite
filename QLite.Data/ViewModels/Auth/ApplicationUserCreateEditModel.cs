using System.ComponentModel;

namespace QLite.Data.ViewModels.Auth
{
    public class ApplicationUserCreateEditModel
    {
        public string Username { get; set; }
        public string Email { get; set; }



        [DisplayName("Is Active")]
        public bool IsActive { get; set; }

        public string Id { get; set; }


        public bool QRless { get; set; } = false;
        public bool Emailless { get; set; } = false;



       
      
    }

}
