using System;
using System.Collections.Generic;

namespace PortalPOC.Models;

public partial class PermissionPolicyRole
{
    public Guid Oid { get; set; }

    public string? Name { get; set; }

    public bool? IsAdministrative { get; set; }

    public bool? CanEditModel { get; set; }

    public int? PermissionPolicy { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public int? ObjectType { get; set; }

    public virtual KappRole? KappRole { get; set; }

    public virtual XpobjectType? ObjectTypeNavigation { get; set; }

    public virtual ICollection<PermissionPolicyActionPermissionObject> PermissionPolicyActionPermissionObjects { get; } = new List<PermissionPolicyActionPermissionObject>();

    public virtual ICollection<PermissionPolicyNavigationPermissionsObject> PermissionPolicyNavigationPermissionsObjects { get; } = new List<PermissionPolicyNavigationPermissionsObject>();

    public virtual ICollection<PermissionPolicyTypePermissionsObject> PermissionPolicyTypePermissionsObjects { get; } = new List<PermissionPolicyTypePermissionsObject>();

    public virtual ICollection<PermissionPolicyUserUsersPermissionPolicyRoleRole> PermissionPolicyUserUsersPermissionPolicyRoleRoles { get; } = new List<PermissionPolicyUserUsersPermissionPolicyRoleRole>();
}
