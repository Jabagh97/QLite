using System;
using System.Collections.Generic;

namespace QLite.Data;

public partial class DashboardDatum
{
    public Guid Oid { get; set; }

    public string? Content { get; set; }

    public string? Title { get; set; }

    public bool? SynchronizeTitle { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }
}
