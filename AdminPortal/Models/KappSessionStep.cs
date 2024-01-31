using System;
using System.Collections.Generic;

namespace PortalPOC.Models;

public partial class KappSessionStep
{
    public Guid Oid { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? CreatedDateUtc { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? ModifiedDateUtc { get; set; }

    public string? Step { get; set; }

    public string? SubStep { get; set; }

    public Guid? KappSession { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }
}
