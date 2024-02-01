using QLite.Data.CustomAttribute;
using QLite.Data.ViewModels.Branch;
using System;
using System.Collections.Generic;

namespace QLite.Data;
//[ModelMapping(typeof(DeskStatus), typeof(DeskStatusViewModel))]

public partial class DeskStatus
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

    public int? DeskActivityStatus { get; set; }

    public DateTime? StateStartTime { get; set; }

    public DateTime? StateEndTime { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }
}
