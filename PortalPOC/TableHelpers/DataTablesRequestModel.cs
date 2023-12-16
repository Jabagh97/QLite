

namespace PortalPOC.TableHelpers
{
    public class DataTablesRequestModel
    {
        public int Draw { get; set; }
        public ColumnModel[]? Columns { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public DSearch? Search { get; set; }
        public DSort? Sort { get; set; }
    }
}
