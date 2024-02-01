using Microsoft.AspNetCore.Mvc;

namespace QLiteDataApi.Helpers
{
    public class DeleteRowsRequest
    {
        public List<string>? SelectedOids { get; set; }
        public string? TabName { get; set; }
        public string? ModelName { get; set; }

        public string? ModelOid { get; set; }
    }
}
