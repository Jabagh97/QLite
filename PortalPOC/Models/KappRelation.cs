using System;
using System.Collections.Generic;

namespace PortalPOC.Models;

public partial class KappRelation
{
    public Guid Oid { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? CreatedDateUtc { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? ModifiedDateUtc { get; set; }

    public Guid? Parent { get; set; }

    public Guid? Child { get; set; }

    public byte[]? Icon { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public virtual KioskApplication? ChildNavigation { get; set; }

    public virtual KioskApplication? ParentNavigation { get; set; }
}
