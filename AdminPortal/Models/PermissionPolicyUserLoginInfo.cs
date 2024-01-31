using System;
using System.Collections.Generic;

namespace PortalPOC.Models;

public partial class PermissionPolicyUserLoginInfo
{
    public Guid Oid { get; set; }

    public string? LoginProviderName { get; set; }

    public string? ProviderUserKey { get; set; }

    public Guid? User { get; set; }

    public int? OptimisticLockField { get; set; }

    public virtual KappUser? UserNavigation { get; set; }
}
