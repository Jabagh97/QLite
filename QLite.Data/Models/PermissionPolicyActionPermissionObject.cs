using System;
using System.Collections.Generic;

namespace QLite.Data;

public partial class PermissionPolicyActionPermissionObject
{
    public Guid Oid { get; set; }

    public string? ActionId { get; set; }

    public Guid? Role { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public virtual PermissionPolicyRole? RoleNavigation { get; set; }
}
