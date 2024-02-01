using NPoco;
using Quavis.Kapp.Data.Dto.ErrorHandling;
using QLite.Common;
using QLite.Data.Adapter.Common;
using QLite.Data.Dto;
using QLite.Data.Dto.Kapp;
using QLite.Data.Model;
using QLite.Data.Model.Common;
using QLite.Data.Model.Kapp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace QLite.Data.Adapter.Kapp
{
    public class KappDataAdapter : BaseDataAdapter, IKappDataAdapter
    {
        public KappDataAdapter(IDbConnectionManager cn)
            : base(cn)
        {
        }

        public LanguageDto GetLanguage(string languageOid)
        {
            return Db.FirstOrDefault<LanguageDto>("select * from Language where Oid=@0 and GCRecord is null ", languageOid);
        }

        public List<LanguageDto> GetLanguages()
        {
            Sql cmd = new(@"select distinct la.* from AccountLanguage ala 
                            inner join Language la on ala.Language = la.Oid                          
                            where ala.GCRecord is null and la.GCRecord is null and ala.Account is null");

            var list = ListByCmd<Language>(cmd);
            if (list == null || list.Count == 0)
                return new List<LanguageDto>();
            return Language.GetDtoCollection<Language, LanguageDto>(list);
        }

        public void Test()
        {
            ListByCmd<Kiosk>(new Sql("select Oid from Kiosk limit 1"));
        }

       

        public List<KappSettingsDto> GetKappSettings(string parameter)
        {
            var brId = QorchUserContext.BranchId;

            //TODO: settings olayını bi test edelim
            var sql = @";with d as (
                            select *, ROW_NUMBER() OVER(PARTITION BY A.Parameter ORDER BY A.pri ) as RowNumber 
                            from (select IIF(Branch is null, 2, 1) pri, Parameter, ParameterValue,
                                Branch, [as].Oid from KappSettings AS [as]
					            where [as].GCRecord is null 
					            and ([as].Branch = @0 or [as].Branch is null)
					            and ([as].Parameter = @1 or @1 is null)
                             ) as A
                        )
                        select * from d  where RowNumber = 1";

            Sql cmd = new(sql, brId, parameter);

            var settings = Db.Fetch<KappSettings>(cmd);
            return KappSettings.GetDtoCollection<KappSettings, KappSettingsDto>(settings);
        }



        public void UpdatePrevSession(string prevSessionId)
        {
            Sql sql = new("update KappSession set EndTime =GETDATE(), EndTimeUtc = GETUTCDATE() where Oid = @0", prevSessionId);
            Execute(sql);
        }



        public List<ResourceDto> GetResources(string? langId = null)
        {
            Sql cmd = new(@"
                select * from (
                    -- Tüm accountlar için ortak resource'lar, eğer account'un kendi resource'u varsa onu alır 
                    select isnull(ar.Oid,r.Oid) Oid,isnull(ar.CreatedBy,r.CreatedBy) CreatedBy,isnull(ar.ModifiedBy,r.ModifiedBy) ModifiedBy,isnull(ar.CreatedDate,r.CreatedDate) CreatedDate,
                    isnull(ar.CreatedDateUtc,r.CreatedDateUtc) CreatedDateUtc,isnull(ar.ModifiedDate,r.ModifiedDate) ModifiedDate,isnull(ar.ModifiedDateUtc,r.ModifiedDateUtc) ModifiedDateUtc,
                    isnull(ar.Account,r.Account) Account,isnull(ar.Language,r.Language) Language,isnull(ar.Parameter,r.Parameter) Parameter,isnull(ar.ParameterValue,r.ParameterValue) ParameterValue,
                    isnull(ar.Description,r.Description) Description,isnull(ar.OptimisticLockField,r.OptimisticLockField) OptimisticLockField,isnull(ar.GCRecord,r.GCRecord) GCRecord 
                    from AccountLanguage ala 
                    join Language la on la.Oid = ala.Language and la.GCRecord is null 
                    join Resource r on r.Language = la.Oid and r.GCRecord is null 
                    left join Resource ar on ar.Language = la.Oid and r.Parameter = ar.Parameter and ar.GCRecord is null and ar.Account = @0 
                    where ala.GCRecord is null and r.Account is null 

                    union all 

                    -- Sadece account'a özel kullanılan ve ortak'ta bulunmayan resource'lar 
                    select ar.*
                    from AccountLanguage ala 
                    join Language la on la.Oid = ala.Language and la.GCRecord is null 
                    join Resource ar on ar.Language = la.Oid and ar.GCRecord is null 
                    left join Resource r on r.Parameter = ar.Parameter and r.Account is null and r.GCRecord is null 
                    where ala.GCRecord is null and r.Parameter is null 
                ) t order by t.Parameter ");

            //Sql cmd = new Sql(@"select r.* from Language la
            //                inner join Resource r on r.Language = la.Oid
            //                left join Company c on r.Company = c.Oid          
            //                where r.GCRecord is null and la.GCRecord is null and c.GCRecord is null and c.Code = @0", alCode);

            if (!string.IsNullOrEmpty(langId))
            {
                cmd.Append(" and la.Oid= @1", langId);
                var str = cmd.SQL;
            }
            var list = ListByCmd<Resource>(cmd);

            var resultList = new List<Resource>();
            if (list == null || list.Count == 0)
                return new List<ResourceDto>();
            foreach (var item in list)
            {
                var resource = resultList.FirstOrDefault(x => x.Parameter == item.Parameter && x.Language == item.Language);
                if (resource != null)
                {
                    if (item.Account != null)
                    {
                        resultList.Remove(resource);
                        resultList.Add(item);
                    }
                }
                else
                {
                    resultList.Add(item);
                }
            }

            return Resource.GetDtoCollection<Resource, ResourceDto>(resultList);
        }




        //public RestApiClient GetApiClient(string clientId)
        //{
        //    Sql cmd = new Sql(@"select convert(nvarchar(50), k.Oid) ClientId, 'platform' ClientType, b.BranchCode KioskAirportCode, null AlappAlCode from kiosk k inner join Branch b
        //                     on k.Branch = b.Oid
        //where k.Oid = @0
        //and k.GCRecord is null and b.GCRecord is null 
        //                    union
        //                    select convert(nvarchar(50), a.Oid) ClientId, 'alapp' ClientType, b.BranchCode KioskAirportCode, al.Code AlappAlCode from KioskApplication a 
        //                        inner join Company al on al.Oid = a.Company
        //                     inner join Kiosk k on k.Oid = a.Kiosk
        //                     inner join Branch b on b.Oid = k.Branch where a.Oid = @0 
        //and a.GCRecord is null and k.GCRecord is null and b.GCRecord is null", clientId);
        //    var client = Db.SingleOrDefault<RestApiClient>(cmd);
        //    return client;
        //}

        public QorchRestApiClient GetApiClient(string kioskHwId)
        {
            Sql cmd = new(@"select k.Oid AppId, b.Oid BranchId,  b.BranchCode, b.Name BranchName, k.KioskType, k.Name                            
                            from Kiosk k                             
	                        inner join Branch b on b.Oid = k.Branch 
                            where k.HwId = @0  
							and k.GCRecord is null and b.GCRecord is null", kioskHwId);
            var client = Db.SingleOrDefault<QorchRestApiClient>(cmd);
            if (client == null)
            {
                throw new Exception($"qorchRestClient coult not be created for params: kmsKiosk:{kioskHwId}");
            }
            return client;
        }




        public Segment GetDefaultSegment(bool throwIfNotFound = true)
        {
            Sql cmd = new(@"select * from Segment where [Default] = 1 and GCRecord is null");
            var sgm = SingleByCmd<Segment>(cmd);

            if (sgm == null && throwIfNotFound)
                throw new Exception("Default Segment not found");
            return sgm;
        }

        public KappUser GetUserInfo(string username)
        {
            Sql cmd = new(@"select cu.*, ppu.StoredPassword, r.Name RoleName, d.Pano, d.DisplayNo, ppu.UserName, d.Name DeskName, b.Name BranchName, d.Macro MacroDesk, b.Macro MacroBranch from KappUser cu
                            left join PermissionPolicyUser ppu on cu.Oid = ppu.Oid
                            left join PermissionPolicyUserUsers_PermissionPolicyRoleRoles ur on ppu.Oid = ur.Users
                            left join PermissionPolicyRole r on ur.Roles = r.Oid
							left join Desk d on d.Oid = cu.Desk
							left join Kiosk k on d.Pano = k.Oid
							left join Branch b on b.Oid = cu.Branch
                            where ppu.UserName=@0 and ppu.GCRecord is null and ppu.GCRecord is null", username);

            var k = Db.FirstOrDefault<KappUser>(cmd);
            if (k != null)
            {
                Sql cmdRoles = new(@"Select * From KappUser cu left join PermissionPolicyUserUsers_PermissionPolicyRoleRoles ppr on cu.Oid = ppr.Users left join KappRole qr on qr.Oid = ppr.Roles Where cu.Oid=@0", k.Oid);
                k.KappRoles = ListByCmd<KappRole>(cmdRoles);
            }
            return k;
        }

        public List<BranchDto> GetBranchList()
        {
            Sql cmd = new(@"select b.* from Branch b where b.GCRecord is null");

            var list = ListByCmd<Branch>(cmd);
            if (list == null || list.Count == 0)
                return new List<BranchDto>();
            return Branch.GetDtoCollection<Branch, BranchDto>(list);
        }

        public Branch GetBranch(string branchId)
        {
            Sql cmd = new(@"select b.*, p.Name as ProvinceName, sp.Name as SubProvinceName, 
                            c.Name as CountryName, a.LogoS as AccountLogo  from [Branch] as b  
                            inner join [Account] as a on a.Oid=b.Account 
                            left join [Country] as c on c.Oid=b.Country
                            left join [Province] as p on p.Oid=b.Province 
							left join [SubProvince] as sp on sp.Oid=b.SubProvince 
                            where b.Oid = @0", branchId);
            var branch = Db.FirstOrDefault<Branch>(cmd);
            return branch;
        }

        public List<DeskDto> GetDeskList(string branchId)
        {
            Sql cmd = new(@"select d.* from Desk d where d.GCRecord is null");
            if (!string.IsNullOrEmpty(branchId))
            {
                cmd.Append(" and Branch= @0", branchId);
            }

            var list = ListByCmd<Desk>(cmd);
            if (list == null || list.Count == 0)
                return new List<DeskDto>();
            return Desk.GetDtoCollection<Desk, DeskDto>(list);
        }

        public List<KappSettingsDto> GetAccountSettings()
        {
            Sql cmd = new(@"select ks.* from KappSettings ks where ks.GCRecord is null");

            var list = ListByCmd<KappSettings>(cmd);
            if (list == null || list.Count == 0)
                return new List<KappSettingsDto>();
            return KappSettings.GetDtoCollection<KappSettings, KappSettingsDto>(list);
        }
        

        public T GetKappParameterValue<T>(string parameter, T defaultValue = default)
        {
            var kappSetting = GetKappSettings(parameter)?.FirstOrDefault();
            if (kappSetting == null)
            {
                return defaultValue ?? default;
            }
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null && converter.CanConvertFrom(typeof(string)) && converter.CanConvertTo(typeof(T)))
            {
                try
                {
                    return (T)converter.ConvertFromString(kappSetting.ParameterValue);
                }
                catch (Exception)
                {
                    LoggerAdapter.Warning(parameter + " type not correct");
                }
            }
            return defaultValue ?? default;
        }

        public Kiosk GetKiosk(string kioskHwId)
        {
            Sql cmd = new(@"select k.*, d.DisplayNo from Kiosk k 
                            inner join Desk d on d.Pano = k.Oid
                            where k.HwId=@0 and k.GCRecord is null", kioskHwId);
            return SingleByCmd<Kiosk>(cmd);
        }

      

       

      
    }
}
