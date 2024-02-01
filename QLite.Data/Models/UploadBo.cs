using System;
using System.Collections.Generic;

namespace QLite.Data;

public partial class UploadBo
{
    public Guid Oid { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }
}
