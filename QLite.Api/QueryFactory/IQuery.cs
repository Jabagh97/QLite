

using QLite.Data;
using QLiteDataApi.Context;

namespace QLiteDataApi.QueryFactory
{

    public interface IQuery
    {
        IQueryable SelectAndJoinQuery(IQueryable? data, Type modelType, Type viewModelType, ApplicationDbContext dbContext, Dictionary<string, object>? optionalArguments = null);
        void CreateInstance(string user, ApplicationDbContext _dbContext, object modelInstance);

        object GetById(string entityId, IQueryable data);

        void UpdateInstance(string user, ApplicationDbContext _dbContext, object existingEntity);

        bool SoftDeleteInstance(ApplicationDbContext _dbContext,IQueryable data, Type modelType, string Oid);

        bool RemoveFromSubList(ApplicationDbContext _dbContext, string tabName, Type modelType, string modelOid, string Oid);


    }
}
