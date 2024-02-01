using System;
using System.Collections.Generic;

namespace QLite.Data;

public partial class TicketPool
{
    public Guid Oid { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? CreatedDateUtc { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? ModifiedDateUtc { get; set; }

    public Guid? Account { get; set; }

    public Guid? Branch { get; set; }

    public Guid? TicketPoolProfile { get; set; }

    public Guid? ServiceType { get; set; }

    public Guid? Segment { get; set; }

    public Guid? KioskApplication { get; set; }

    public int? MaxWaitingTicketCount { get; set; }

    public DateTime? MaxWaitingTicketCountControlTime { get; set; }

    public DateTime? ServiceStartTime { get; set; }

    public DateTime? ServiceEndTime { get; set; }

    public DateTime? BreakStartTime { get; set; }

    public DateTime? BreakEndTime { get; set; }

    public int? RangeStart { get; set; }

    public int? RangeEnd { get; set; }

    public bool? ResetOnRange { get; set; }

    public bool? NotAvailable { get; set; }

    public string? ServiceCode { get; set; }

    public int? CopyNumber { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public virtual Account? AccountNavigation { get; set; }

    public virtual Branch? BranchNavigation { get; set; }

    public virtual KioskApplication? KioskApplicationNavigation { get; set; }

    public virtual Segment? SegmentNavigation { get; set; }

    public virtual ServiceType? ServiceTypeNavigation { get; set; }

    public virtual TicketPoolProfile? TicketPoolProfileNavigation { get; set; }
}
