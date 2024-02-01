using System;
using System.Collections.Generic;

namespace QLite.Data;

public partial class XpweakReference
{
    public Guid Oid { get; set; }

    public int? TargetType { get; set; }

    public string? TargetKey { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public int? ObjectType { get; set; }

    public virtual ICollection<AuditDataItemPersistent> AuditDataItemPersistentNewObjectNavigations { get; } = new List<AuditDataItemPersistent>();

    public virtual ICollection<AuditDataItemPersistent> AuditDataItemPersistentOldObjectNavigations { get; } = new List<AuditDataItemPersistent>();

    public virtual AuditedObjectWeakReference? AuditedObjectWeakReference { get; set; }

    public virtual XpobjectType? ObjectTypeNavigation { get; set; }

    public virtual XpobjectType? TargetTypeNavigation { get; set; }
}
