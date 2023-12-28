using PortalPOC.Models;

namespace PortalPOC.QueryFactory
{
    public interface IQueryFactory
    {
        IQueryable SelectQuery(Type modelType, QuavisQorchAdminEasyTestContext dbContext);
    }
}
