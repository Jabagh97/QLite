
namespace PortalPOC.Helpers
{

      public class DeleteRowsRequest
    {
        public List<string>? SelectedOids { get; set; }
        public string? TabName { get; set; }
        public string? ModelName { get; set; }

        public string? ModelOid { get; set;}
    }
    public class DataTableRequestParameters
    {
        public int PageSize { get; set; }
        public int Skip { get; set; }
        public string? SearchValue { get; set; }
        public string? SortColumn { get; set; }
        public string? SortColumnDirection { get; set; }
    }

    public interface IDataTableRequestExtractor
    {
        DataTableRequestParameters ExtractParameters(IFormCollection formData);
    }

    public class DataTableRequestExtractor : IDataTableRequestExtractor
    {
        public DataTableRequestParameters ExtractParameters(IFormCollection formData)
        {
            return new DataTableRequestParameters
            {
                PageSize = int.TryParse(formData["length"], out var lengthValue) ? lengthValue : 0,
                Skip = int.TryParse(formData["start"], out var startValue) ? startValue : 0,
                SearchValue = formData["search[value]"],
                SortColumn = formData[string.Concat("columns[", formData["order[0][column]"], "][name]")],
                SortColumnDirection = formData["order[0][dir]"]
            };
        }
    }
}
