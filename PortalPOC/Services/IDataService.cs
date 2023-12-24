namespace PortalPOC.Services
{
    public interface IDataService
    {
        IQueryable GetTypedDbSet(Type modelType);
        IQueryable ApplySearchFilter(IQueryable data, string searchValue,Type modelType);
        IQueryable ApplySorting(IQueryable data, string sortColumn, string sortColumnDirection, Type modelType);

        IQueryable GetFilteredAndPaginatedData(Type modelType, Type viewModelType, IQueryable data, string searchValue, string sortColumn, string sortColumnDirection, Dictionary<string, (Type, Type)> modelTypeMapping);

    }
}
