using System;
using System.Collections.Generic;

namespace QLite.Data;

public partial class TicketPoolProfile
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

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public virtual Account? AccountNavigation { get; set; }

    public virtual ICollection<Branch> Branches { get; } = new List<Branch>();

    public virtual ICollection<TicketPool> TicketPools { get; } = new List<TicketPool>();
}
