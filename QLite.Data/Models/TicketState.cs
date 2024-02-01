using System;
using System.Collections.Generic;

namespace QLite.Data;

public partial class TicketState
{
    public Guid Oid { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? CreatedDateUtc { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? ModifiedDateUtc { get; set; }

    public Guid? Desk { get; set; }

    public Guid? User { get; set; }

    public Guid? Ticket { get; set; }

    public Guid? Branch { get; set; }

    public int? TicketNumber { get; set; }

    public int? TicketStateValue { get; set; }

    public int? TicketOprValue { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public Guid? ServiceType { get; set; }

    public Guid? Segment { get; set; }

    public string? ServiceTypeName { get; set; }

    public string? SegmentName { get; set; }

    public Guid? Macro { get; set; }

    public string? CallingRuleDescription { get; set; }

    public int? DeskAppType { get; set; }

    public int? TicketCallType { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public Guid? KioskAppId { get; set; }

    public virtual Branch? BranchNavigation { get; set; }

    public virtual Desk? DeskNavigation { get; set; }

    public virtual Segment? SegmentNavigation { get; set; }

    public virtual ServiceType? ServiceTypeNavigation { get; set; }

    public virtual Ticket? TicketNavigation { get; set; }
}
