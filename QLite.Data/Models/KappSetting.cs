﻿using System;
using System.Collections.Generic;

namespace QLite.Data;

public partial class KappSetting
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

    public Guid? KioskApplication { get; set; }

    public string? Parameter { get; set; }

    public string? ParameterValue { get; set; }

    public string? Description { get; set; }

    public int? CacheTimeout { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public virtual Account? AccountNavigation { get; set; }

    public virtual Branch? BranchNavigation { get; set; }

    public virtual KioskApplication? KioskApplicationNavigation { get; set; }
}