namespace QLiteDataApi.Services
{
    public interface IDataService
    {


        IQueryable GetFilteredAndPaginatedData(Type modelType, Type viewModelType, string? searchValue, string? sortColumn, string? sortColumnDirection);

        Dictionary<string, List<dynamic>> GetGuidPropertyNames(Type modelType, Dictionary<string, (Type, Type)> modelTypeMapping);

        object CreateModel(string user, Type modelType, Dictionary<string, object> formData);
        object UpdateModel(string user, Type modelType, Dictionary<string, object> formData);

        bool SoftDelete(Type modelType, Dictionary<string, object> formData);

        IQueryable GetTabData(Type innerType, Type innerViewType, string Oid, Type mainModelType);

        bool RemoveFromSubList(string tabName, Type modelType, string modelOid, List<string> Oids);
    }
}
