using System;
using System.Collections.Generic;

namespace PortalPOC.Models;

public partial class XpobjectType
{
    public int Oid { get; set; }

    public string? TypeName { get; set; }

    public string? AssemblyName { get; set; }

    public virtual ICollection<PermissionPolicyRole> PermissionPolicyRoles { get; } = new List<PermissionPolicyRole>();

    public virtual ICollection<PermissionPolicyUser> PermissionPolicyUsers { get; } = new List<PermissionPolicyUser>();

    public virtual ICollection<XpweakReference> XpweakReferenceObjectTypeNavigations { get; } = new List<XpweakReference>();

    public virtual ICollection<XpweakReference> XpweakReferenceTargetTypeNavigations { get; } = new List<XpweakReference>();
}
