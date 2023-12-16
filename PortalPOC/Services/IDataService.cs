namespace PortalPOC.Services
{
    public interface IDataService
    {
        IEnumerable<object> GetTypedDbSet(Type modelType);
        IEnumerable<object> ApplySearchFilter(IEnumerable<object> data, string searchValue);
        IEnumerable<object> ApplySorting(IEnumerable<object> data, string sortColumn, string sortColumnDirection, Type modelType);
    }
}
