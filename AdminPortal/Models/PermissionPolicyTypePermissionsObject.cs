using System;
using System.Collections.Generic;

namespace PortalPOC.Models;

public partial class PermissionPolicyTypePermissionsObject
{
    public Guid Oid { get; set; }

    public Guid? Role { get; set; }

    public string? TargetType { get; set; }

    public int? ReadState { get; set; }

    public int? WriteState { get; set; }

    public int? CreateState { get; set; }

    public int? DeleteState { get; set; }

    public int? NavigateState { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public virtual ICollection<PermissionPolicyMemberPermissionsObject> PermissionPolicyMemberPermissionsObjects { get; } = new List<PermissionPolicyMemberPermissionsObject>();

    public virtual ICollection<PermissionPolicyObjectPermissionsObject> PermissionPolicyObjectPermissionsObjects { get; } = new List<PermissionPolicyObjectPermissionsObject>();

    public virtual PermissionPolicyRole? RoleNavigation { get; set; }
}
