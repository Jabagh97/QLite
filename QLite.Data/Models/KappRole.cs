using System;
using System.Collections.Generic;

namespace QLite.Data;

public partial class KappRole
{
    public Guid Oid { get; set; }

    public int? BusinessRole { get; set; }

    public virtual PermissionPolicyRole OidNavigation { get; set; } = null!;
}
