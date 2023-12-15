using System;
using System.Collections.Generic;

namespace PortalPOC.Models;

public partial class QorchSession
{
    public Guid Oid { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? CreatedDateUtc { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? ModifiedDateUtc { get; set; }

    public Guid? Account { get; set; }

    public Guid? KioskApplication { get; set; }

    public Guid? Segment { get; set; }

    public Guid? ServiceType { get; set; }

    public bool? Success { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? StartTimeUtc { get; set; }

    public DateTime? EndTime { get; set; }

    public DateTime? EndTimeUtc { get; set; }

    public string? CurrentStep { get; set; }

    public string? InputValue { get; set; }

    public string? InputInfo { get; set; }

    public string? InputType { get; set; }

    public Guid? Workflow { get; set; }

    public string? Error { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public virtual Account? AccountNavigation { get; set; }

    public virtual KioskApplication? KioskApplicationNavigation { get; set; }

    public virtual Segment? SegmentNavigation { get; set; }

    public virtual ServiceType? ServiceTypeNavigation { get; set; }
}
