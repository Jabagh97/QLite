namespace PortalPOC.Services
{
    public interface IDataService
    {
        IQueryable<object> GetTypedDbSet(Type modelType);
        IQueryable<object> ApplySearchFilter(IQueryable<object> data, string searchValue);
        IQueryable<object> ApplySorting(IQueryable<object> data, string sortColumn, string sortColumnDirection, Type modelType);

        IQueryable GetFilteredAndPaginatedData(Type modelType, Type viewModelType, IQueryable<object> data, string searchValue, string sortColumn, string sortColumnDirection, Dictionary<string, (Type, Type)> modelTypeMapping);

    }
}
