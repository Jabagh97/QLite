using System;
using System.Collections.Generic;

namespace PortalPOC.Models;

public partial class DeskTransferableService
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

    public Guid? Desk { get; set; }

    public Guid? ServiceType { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public virtual Branch? BranchNavigation { get; set; }

    public virtual Desk? DeskNavigation { get; set; }

    public virtual ServiceType? ServiceTypeNavigation { get; set; }
}
