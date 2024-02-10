using System.ComponentModel.DataAnnotations;

namespace QLite.Data.ViewModels.SubProvince
{
    internal class SubProvinceViewModel
    {

        public string? Country { get; set; }

        public string? Province { get; set; }
        [Required]

        public string? Name { get; set; }

    }
}