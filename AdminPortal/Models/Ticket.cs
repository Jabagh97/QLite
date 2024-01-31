using System;
using System.Collections.Generic;

namespace PortalPOC.Models;

public partial class Ticket
{
    public Guid Oid { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? CreatedDateUtc { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? ModifiedDateUtc { get; set; }

    public Guid? Branch { get; set; }

    public Guid? ServiceType { get; set; }

    public Guid? Desk { get; set; }

    public Guid? Segment { get; set; }

    public string? ServiceTypeName { get; set; }

    public string? SegmentName { get; set; }

    public string? LangCode { get; set; }

    public int? CurrentState { get; set; }

    public int? LastOpr { get; set; }

    public DateTime? LastOprTime { get; set; }

    public Guid? ToServiceType { get; set; }

    public Guid? ToDesk { get; set; }

    public Guid? CurrentDesk { get; set; }

    public int? Number { get; set; }

    public int? DayOfYear { get; set; }

    public int? Year { get; set; }

    public string? TicketNote { get; set; }

    public string? CustomerInfo { get; set; }

    public string? CustomerNo { get; set; }

    public string? CardNo { get; set; }

    public string? NationalId { get; set; }

    public Guid? TicketPool { get; set; }

    public Guid? CreatedByDesk { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public Guid? CreatedByKiosk { get; set; }

    public virtual Branch? BranchNavigation { get; set; }

    public virtual Desk? CurrentDeskNavigation { get; set; }

    public virtual Desk? DeskNavigation { get; set; }

    public virtual Segment? SegmentNavigation { get; set; }

    public virtual ServiceType? ServiceTypeNavigation { get; set; }

    public virtual ICollection<TicketState> TicketStates { get; } = new List<TicketState>();

    public virtual Desk? ToDeskNavigation { get; set; }

    public virtual ServiceType? ToServiceTypeNavigation { get; set; }
}
