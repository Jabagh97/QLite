using System;
using System.Collections.Generic;

namespace PortalPOC.Models;

public partial class KappRole
{
    public Guid Oid { get; set; }

    public int? BusinessRole { get; set; }

    public virtual PermissionPolicyRole OidNavigation { get; set; } = null!;
}
