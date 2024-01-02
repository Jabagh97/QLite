using Microsoft.EntityFrameworkCore;
using PortalPOC.Models;

namespace PortalPOC.QueryFactory
{
    public interface IQueryFactory
    {
        IQueryable SelectAndJoinQuery(IQueryable? data, Type modelType, Type viewModelType, QuavisQorchAdminEasyTestContext dbContext);
        void CreateInstance(QuavisQorchAdminEasyTestContext _dbContext, object modelInstance);

        object GetById(string entityId, IQueryable data);

        void UpdateInstance(QuavisQorchAdminEasyTestContext _dbContext, object existingEntity);

        bool SoftDeleteInstance(QuavisQorchAdminEasyTestContext _dbContext,IQueryable data, Type modelType, string Oid);
    }
}
