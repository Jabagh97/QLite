using System;
using System.Collections.Generic;

namespace PortalPOC.Models;

public partial class KappWorkflow
{
    public Guid Oid { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? CreatedDateUtc { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? ModifiedDateUtc { get; set; }

    public string? Name { get; set; }

    public string? SessionType { get; set; }

    public string? DesignData { get; set; }

    public Guid? RestartProfile { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public virtual ICollection<KioskApplication> KioskApplications { get; } = new List<KioskApplication>();
}
