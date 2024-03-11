using System.ComponentModel.DataAnnotations;

namespace QLite.Data.ViewModels.KappSetting
{
    internal class KappSettingViewModel
    {

        public string? Account { get; set; }

        public string? Branch { get; set; }

        public string? Kiosk { get; set; }
        [Required]

        public string? Parameter { get; set; }
        [Required]

        public string? ParameterValue { get; set; }

        public string? Description { get; set; }

        public int? CacheTimeout { get; set; }
    }
}