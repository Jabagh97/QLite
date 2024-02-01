using System;
using System.Collections.Generic;

namespace QLite.Data;

public partial class RestartProfile
{
    public Guid Oid { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? CreatedDateUtc { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? ModifiedDateUtc { get; set; }

    public string? Name { get; set; }

    public int? RestartPerNumOfSession { get; set; }

    public int? RestartPerNumOfDays { get; set; }

    public DateTime? RestartTime { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }
}
