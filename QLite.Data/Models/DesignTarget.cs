using QLite.Data.CustomAttribute;
using QLite.Data.ViewModels.Design;
using QLite.Data.ViewModels.DesignTarget;
using System;
using System.Collections.Generic;

namespace QLite.Data;
[ModelMapping(typeof(DesignTarget), typeof(DesignTargetViewModel))]

public partial class DesignTarget
{
    public Guid Oid { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? CreatedDateUtc { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? ModifiedDateUtc { get; set; }

    public Guid? Design { get; set; }

    public Guid? Account { get; set; }

    public Guid? Branch { get; set; }

    public Guid? Kiosk { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public virtual Design? DesignNavigation { get; set; }
}
