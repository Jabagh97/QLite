

using Quavis.QorchLite.Data.Dto.Kapp;

namespace Quavis.QorchLite.Data.Dto
{
    public class SegmentDto: KappBaseDto
    {
        public string Name { get; set; }
        public bool Default { get; set; }
        public string Prefix { get; set; }

    }
}
