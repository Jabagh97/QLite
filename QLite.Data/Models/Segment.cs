using QLite.Data.CustomAttribute;
using QLite.Data.ViewModels.Resource;
using QLite.Data.ViewModels.Segment;
using System;
using System.Collections.Generic;

namespace QLite.Data;
[ModelMapping(typeof(Segment), typeof(SegmentViewModel))]

public partial class Segment
{
    public Guid Oid { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? CreatedDateUtc { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? ModifiedDateUtc { get; set; }

    public Guid? Account { get; set; }

    public string? Name { get; set; }

    public bool? Default { get; set; }

    public bool? IsParent { get; set; }

    public Guid? Parent { get; set; }

    public string? Prefix { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public virtual Account? AccountNavigation { get; set; }


    public virtual ICollection<Segment> InverseParentNavigation { get; } = new List<Segment>();

    public virtual ICollection<MacroRule> MacroRules { get; } = new List<MacroRule>();

    public virtual Segment? ParentNavigation { get; set; }

    public virtual ICollection<QorchSession> QorchSessions { get; } = new List<QorchSession>();

    public virtual ICollection<TicketPool> TicketPools { get; } = new List<TicketPool>();

    public virtual ICollection<TicketState> TicketStates { get; } = new List<TicketState>();

    public virtual ICollection<Ticket> Tickets { get; } = new List<Ticket>();
}
