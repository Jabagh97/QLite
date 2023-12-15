using System;
using System.Collections.Generic;

namespace PortalPOC.Models;

public partial class MacroRule
{
    public Guid Oid { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? CreatedDateUtc { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? ModifiedDateUtc { get; set; }

    public Guid? Macro { get; set; }

    public Guid? ServiceType { get; set; }

    public Guid? Segment { get; set; }

    public bool? Transfer { get; set; }

    public int? ToThisDesk { get; set; }

    public int? MaxWaitingTime { get; set; }

    public int? MinWaitingTime { get; set; }

    public int? Sequence { get; set; }

    public string? Description { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public virtual Macro? MacroNavigation { get; set; }

    public virtual Segment? SegmentNavigation { get; set; }

    public virtual ServiceType? ServiceTypeNavigation { get; set; }
}
