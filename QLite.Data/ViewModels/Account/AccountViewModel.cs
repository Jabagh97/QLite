using QLite.Data.CustomAttribute;
using System.ComponentModel.DataAnnotations;

namespace QLite.Data.ViewModels.Account
{
    internal class AccountViewModel
    {
        [Required]

        public string? Name { get; set; }

        public string? Code { get; set; }

        public string? Mail { get; set; }
        [IconAttribute]

        public byte[]? LogoS { get; set; }


    }
}