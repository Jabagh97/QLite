using System;
using System.Collections.Generic;

namespace PortalPOC.Models;

public partial class AuditedObjectWeakReference
{
    public Guid Oid { get; set; }

    public Guid? GuidId { get; set; }

    public int? IntId { get; set; }

    public string? DisplayName { get; set; }

    public virtual ICollection<AuditDataItemPersistent> AuditDataItemPersistents { get; } = new List<AuditDataItemPersistent>();

    public virtual XpweakReference OidNavigation { get; set; } = null!;
}
