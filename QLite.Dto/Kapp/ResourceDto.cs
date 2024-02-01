using System;

namespace QLite.Dto.Kapp
{
    public class ResourceDto : KappBaseDto
    {
        public string Parameter { get; set; }
        public string ParameterValue { get; set; }
        public string Language { get; set; }
        public string Description { get; set; }
    }
}
