using PortalPOC.Models;

namespace PortalPOC.QueryFactory
{
    public interface IQueryFactory
    {
        IQueryable SelectAndJoinQuery(Type modelType, QuavisQorchAdminEasyTestContext dbContext);
    }
}
