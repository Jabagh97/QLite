namespace PortalPOC.Services
{
    public interface IDataService
    {
        IQueryable? GetTypedDbSet(Type modelType);
        IQueryable ApplySearchFilter(IQueryable data, string? searchValue,Type modelType, Type viewModelType);
        IQueryable ApplySorting(IQueryable data, string? sortColumn, string? sortColumnDirection);

        IQueryable GetFilteredAndPaginatedData(Type modelType, Type viewModelType, string? searchValue, string? sortColumn, string? sortColumnDirection, Dictionary<string, (Type, Type)> modelTypeMapping);

        Dictionary<string, List<dynamic>> GetGuidPropertyNames(Type modelType, Dictionary<string, (Type, Type)> modelTypeMapping);
    }
}
