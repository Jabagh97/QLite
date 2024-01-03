using PortalPOC.Models;

namespace PortalPOC.QueryFactory
{
    public interface ISpecialQueryFactory : IQueryFactory
    {
        // Add additional methods specific to the special table
    }

    public class SpecialQueryFactory : ISpecialQueryFactory
    {
        // Implement the specialized methods
        public void CreateInstance(QuavisQorchAdminEasyTestContext _dbContext, object modelInstance)
        {
            throw new NotImplementedException();
        }

        public object GetById(string entityId, IQueryable data)
        {
            throw new NotImplementedException();
        }

        public IQueryable SelectAndJoinQuery(IQueryable? data, Type modelType, Type viewModelType, QuavisQorchAdminEasyTestContext dbContext)
        {
            throw new NotImplementedException();
        }

        public bool SoftDeleteInstance(QuavisQorchAdminEasyTestContext _dbContext, IQueryable data, Type modelType, string Oid)
        {
            throw new NotImplementedException();
        }

        public void UpdateInstance(QuavisQorchAdminEasyTestContext _dbContext, object existingEntity)
        {
            throw new NotImplementedException();
        }
    }
}
