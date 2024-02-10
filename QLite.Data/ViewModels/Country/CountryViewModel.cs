using QLite.Data.CustomAttribute;
using System.ComponentModel.DataAnnotations;

namespace QLite.Data.ViewModels.Country
{
    internal class CountryViewModel 
    {
        [Required]
        public string? Name { get; set; }

        public string? Mask { get; set; }


        public string? Sequence { get; set; }


        public string? PhoneCode { get; set; }



        public string? LangCode { get; set; }
        [Icon]

        public string? Logo { get; set; }
      

    }
}