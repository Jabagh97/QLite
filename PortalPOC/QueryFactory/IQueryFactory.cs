using PortalPOC.Models;

namespace PortalPOC.QueryFactory
{
    public interface IQueryFactory
    {
        IQueryable SelectAndJoinQuery(IQueryable? data, Type modelType, Type viewModelType, QuavisQorchAdminEasyTestContext dbContext);
    }
}
