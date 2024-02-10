using QLite.Data.CustomAttribute;
using System.ComponentModel.DataAnnotations;

namespace QLite.Data.ViewModels.Language
{
    internal class LanguageViewModel
    {
        [Required]

        public string? Name { get; set; }

        public string? EnglishName { get; set; }

        public string? LocalName { get; set; }

        public string? CultureInfo { get; set; }

        public string? LangCode { get; set; }
        [IconAttribute]

        public byte[]? Logo { get; set; }
        [Boolean]

        public bool? IsDefault { get; set; }

    }
}