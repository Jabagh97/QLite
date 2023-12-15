using System;
using System.Collections.Generic;

namespace PortalPOC.Models;

public partial class DeskMacroSchedule
{
    public Guid Oid { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? CreatedDateUtc { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? ModifiedDateUtc { get; set; }

    public Guid? Macro { get; set; }

    public Guid? Account { get; set; }

    public Guid? Branch { get; set; }

    public Guid? Desk { get; set; }

    public Guid? User { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public bool? HaftalikRutin { get; set; }

    public bool? D1 { get; set; }

    public bool? D2 { get; set; }

    public bool? D3 { get; set; }

    public bool? D4 { get; set; }

    public bool? D5 { get; set; }

    public bool? D6 { get; set; }

    public bool? D7 { get; set; }

    public bool? Pasif { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public virtual Branch? BranchNavigation { get; set; }

    public virtual Desk? DeskNavigation { get; set; }

    public virtual Macro? MacroNavigation { get; set; }

    public virtual KappUser? UserNavigation { get; set; }
}
