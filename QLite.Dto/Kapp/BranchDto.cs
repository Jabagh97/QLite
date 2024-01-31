namespace Quavis.QorchLite.Data.Dto.Kapp
{

    public class BranchDto: KappBaseDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string CountryName { get; set; }
        public string ProvinceName { get; set; }
        public string SubProvinceName { get; set; }
        public string Address { get; set; }
    }
}
