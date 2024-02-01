using NPoco;
using NPoco.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QLite.Data.Adapter.Common
{
    public class BaseDataAdapter
    {
        protected IDbConnectionManager Cn;

        protected IDatabase Db => lazyDb.Value;

        private readonly Lazy<IDatabase> lazyDb;

        internal BaseDataAdapter(IDbConnectionManager cn)
        {
            Cn = cn;
            lazyDb = new Lazy<IDatabase>(() => Cn.Db);
        }

        protected IQueryProviderWithIncludes<T> Query<T>()
        {
            return Db.Query<T>();
        }

        public void Save<T>(T model) where T : BaseModel
        {
            if (Db.IsNew(model))
            {
                model.CreatedBy = QorchUserContext.Name;
                model.CreatedDate = DateTime.Now;
                model.CreatedDateUtc = DateTime.UtcNow;
            }
            model.ModifiedBy = QorchUserContext.Name;
            if (model.ModifiedDate == null)
                model.ModifiedDate = DateTime.Now;
            if (model.ModifiedDateUtc == null)
                model.ModifiedDateUtc = DateTime.UtcNow;
            if (string.IsNullOrEmpty(model.Oid))
                model.Oid = Guid.NewGuid().ToString();
            Db.Save(model);
        }

        public void Insert<T>(T model) where T : BaseModel
        {
            if (string.IsNullOrEmpty(model.Oid))
                model.Oid = Guid.NewGuid().ToString();
            model.CreatedBy = QorchUserContext.Name;
            model.CreatedDate = DateTime.Now;
            model.CreatedDateUtc = DateTime.UtcNow;
            model.ModifiedBy = QorchUserContext.Name;
            model.ModifiedDate = DateTime.Now;
            model.ModifiedDateUtc = DateTime.UtcNow;

            Db.Insert<T>(model);
        }

        public void Update<T>(T model)
        {
            if (model is BaseModel)
            {
                (model as BaseModel).ModifiedBy = QorchUserContext.Name;
                (model as BaseModel).ModifiedDate = DateTime.Now;
                (model as BaseModel).ModifiedDateUtc = DateTime.UtcNow;
            }
            Db.Update(model);
        }

        public T FirstOrDefault<T>(System.Linq.Expressions.Expression<Func<T, bool>> filter = null) where T : BaseModel
        {
            T ret = Db.Query<T>().FirstOrDefault(filter);
            return ret;
        }

        public List<T> List<T>()
        {
            //filter = BuildFilter(filter);
            return Db.Fetch<T>();
        }

        public List<T> List<T>(string sql, params object[] args)
        {
            //filter = BuildFilter(filter);
            return Db.Fetch<T>(sql, args);
        }

        public List<T> List<T>(System.Linq.Expressions.Expression<Func<T, bool>> filter)
        {
            List<T> lst = Db.Query<T>().Where(filter).ToList();
            return lst;
        }

        protected List<T> ListByCmd<T>(Sql sqlCmd)
        {
            if (sqlCmd == null)
                return Db.Fetch<T>().ToList();
            else
                return Db.Fetch<T>(sqlCmd).ToList();
        }

        public T SingleByCmd<T>(Sql sqlCmd, bool throwIfException = true)
        {
            if (sqlCmd == null)
                throw new Exception("sqlCmd can not be empty");

            try
            {
                return Db.SingleOrDefault<T>(sqlCmd);
            }
            catch (Exception ex)
            {
                LoggerAdapter.Error(ex, "singleOrDefult:" + sqlCmd.SQL);
                if (throwIfException)
                    throw;
                else
                {
                    var list = Db.Fetch<T>().ToList();
                    return list.FirstOrDefault();
                }
            }
        }

        public T SingleById<T>(string id) where T : BaseModel
        {
            //if (id == null)
            //    throw new Exception("id can not be empty");

            Sql sql = new("Oid = @0 and GCRecord is null", id);
            return Db.Query<T>().Where(x => x.Oid == id && x.GCRecord == null).SingleOrDefault();
        }

        public T SingleByIdTest<T>(string id) where T : BaseModel
        {
            return Db.SingleOrDefaultById<T>(id);
        }

        public int Execute(Sql sql)
        {
            return Db.Execute(sql);
        }

        #region user

        //public QUser GetUser(string OId)
        //{
        //    NPoco.Sql cmd = new NPoco.Sql(@"select u.*, ppu.UserName, ppu.StoredPassword, d.Pano, p.HwId DeskPanoHwId, d.Name DeskName, null npoco_BranchObj, b.* from QUser u
        //                                        inner join PermissionPolicyUser ppu on u.Oid = ppu.Oid
        //                                        left join Desk d on d.Oid = u.Desk and d.GCRecord is null
        //                                        left join Pano p on d.Pano = p.Oid
        //                                        left join Branch b on u.Branch = b.Oid
        //                                        where ppu.Oid = @0 and ppu.GCRecord is null", OId);
        //    QUser model = db.SingleOrDefault<QUser>(cmd);

        //    if (model != null)
        //        LoadRoles(model);

        //    return model;
        //}

        //public void LoadRoles(QUser u)
        //{
        //    NPoco.Sql cmd = new NPoco.Sql(@"select r.*, qr.BusinessRole from PermissionPolicyUserUsers_PermissionPolicyRoleRoles ur
        //                                    inner join PermissionPolicyRole r on r.Oid = ur.Roles
        //                                    inner join QRole qr on qr.Oid = r.Oid
        //                                            where ur.Users = @0 and r.GCRecord is null", u.Oid);
        //    u.RoleList = ListByCmd<QRole>(cmd);
        //}

        //public List<QUser> ListBranchUsers()
        //{
        //    Sql cmd = new Sql(@"select u.Oid, ppu.UserName, null npoco_DeskObj, d.* from QUser u
        //                        inner join PermissionPolicyUser ppu on u.Oid = ppu.Oid
        //                        left join Desk d on u.Desk = d.Oid
        //                        where ppu.GCRecord is null and u.Branch =@0", UserContext.BranchOId);//s.Branch='" + UserContext.BranchOId.ToString() + "' and
        //    List<QUser> lst = ListByCmd<QUser>(cmd);
        //    return lst;
        //}

        #endregion user

    }
}
