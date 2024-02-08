using static QLite.Data.Models.Enums;
using System.ComponentModel;
using System.Security.Claims;
using QLite.CommonConstant;
using System;
using System.Threading;
using System.Collections.Generic;

namespace QLite.CommonContext
{
    public class UserContext
    {

        public static string Name
        {
            get
            {
                return GetClaim<string>(ClaimTypes.Name);
            }
        }

        public static string HwId
        {
            get
            {
                return GetClaim<string>(AppClaimTypes.KioskHwIdClaimType);
            }
        }

        public static string AccountCode
        {
            get
            {
                return GetClaim<string>(AppClaimTypes.AccountCodeClaimType);
            }
        }

        public static string AppCode
        {
            get
            {
                return GetClaim<string>(AppClaimTypes.AppCodeClaimType);
            }
        }

        public static string AppDesignTag
        {
            get
            {
                return GetClaim<string>(AppClaimTypes.AppDesignTagClaimType);
            }
        }

        public static Guid? AppId
        {
            get
            {
                return GetClaim<Guid>(AppClaimTypes.AppIdClaimType);
            }
        }

        public static Guid? KappTypeId
        {
            get
            {
                return GetClaim<Guid>(AppClaimTypes.KappTypeIdClaimType);
            }
        }

        public static string ClientType
        {
            get
            {
                return GetClaim<string>(AppClaimTypes.ClientTypeClaimType);
            }
        }

        public static string DeskPanoId
        {
            get
            {
                return GetClaim<string>(AppClaimTypes.DeskPanoIdClaimType);
            }
        }

        public static string DeskDisplayNo
        {
            get
            {
                return GetClaim<string>(AppClaimTypes.DeskDisplayNoClaimType);
            }
        }

        public static void UpdateClaim(string claimType, string value)
        {
            Claim c = (Thread.CurrentPrincipal as ClaimsPrincipal).FindFirst(claimType);
            if (c != null)
                (Thread.CurrentPrincipal.Identity as ClaimsIdentity).RemoveClaim(c);

            (Thread.CurrentPrincipal.Identity as ClaimsIdentity).AddClaim(new Claim(claimType, value));
        }

        private static T GetClaim<T>(string claimType)
        {
            if (Thread.CurrentPrincipal == null)
                return default(T);

            Claim c = (Thread.CurrentPrincipal as ClaimsPrincipal).FindFirst(claimType);
            var converter = TypeDescriptor.GetConverter(typeof(T));

            if (c == null)
                return default(T);

            if (converter != null)
            {
                return (T)converter.ConvertFromString(c.Value);
            }
            else
                throw new Exception("TypeConverter for given type:" + typeof(T).Name);

         
        }

        public static Guid? DeskId
        {
            get
            {
                return GetClaim<Guid?>(AppClaimTypes.DeskIdClaimType);
            }
        }

        public static string DeskName
        {
            get
            {
                return GetClaim<string>(AppClaimTypes.DeskNameClaimType);
            }
        }

        public static string DeskUserName
        {
            get
            {
                return GetClaim<string>(AppClaimTypes.UserNameClaimType);
            }
        }



        public static Guid? BranchId
        {
            get
            {
                return GetClaim<Guid?>(AppClaimTypes.BranchIdClaimType);

            }
        }

        public static Guid AccountId
        {
            get
            {
                return GetClaim<Guid>(AppClaimTypes.AccountIdClaimType);
            }
        }


        public static string BranchName
        {
            get
            {
                return GetClaim<string>(AppClaimTypes.BranchNameClaimType);

            }
        }

        public static Guid? UserId
        {
            get
            {
                return GetClaim<Guid?>(AppClaimTypes.UserIdClaimType);
            }
        }

        public static Guid SessionId
        {
            get
            {
                return GetClaim<Guid>(AppClaimTypes.SessionIdType);

            }
        }

        public static DeskAppType DeskAppType
        {
            get
            {
                return GetClaim<DeskAppType>(AppClaimTypes.DeskAppTypeClaimType);

            }
        }

        public static IEnumerable<Claim> GetAllClaims()
        {
            return (Thread.CurrentPrincipal as ClaimsPrincipal).Claims;
        }
    }

}
