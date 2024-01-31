using System.Collections.Generic;

namespace Quavis.QorchLite.Data.Dto.Kapp
{
    public class LanguageDto : KappBaseDto
    {
        public string Name { get; set; }
        public string EnglishName { get; set; }
        public string LocalName { get; set; }
        public string CultureInfo { get; set; }
        public string LangCode { get; set; }
        public byte[] Logo { get; set; }
        public bool IsDefault { get; set; }

        public List<ResourceDto> Resources { get; set; }
    }
}
