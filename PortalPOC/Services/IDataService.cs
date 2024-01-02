namespace PortalPOC.Services
{
    public interface IDataService
    {
      

        IQueryable GetFilteredAndPaginatedData(Type modelType, Type viewModelType, string? searchValue, string? sortColumn, string? sortColumnDirection, Dictionary<string, (Type, Type)> modelTypeMapping);

        Dictionary<string, List<dynamic>> GetGuidPropertyNames(Type modelType, Dictionary<string, (Type, Type)> modelTypeMapping);

        object CreateModel(Type modelType, Dictionary<string, object> formData);
        object UpdateModel(Type modelType, Dictionary<string, object> formData);

        bool SoftDelete(Type modelType, Dictionary<string, object> formData);
    }
}
