using QLite.Data.CustomAttribute;
using QLite.Data.ViewModels.Resource;
using System;
using System.Collections.Generic;

namespace QLite.Data;
[ModelMapping(typeof(Resource), typeof(ResourceViewModel))]

public partial class Resource
{
    public Guid Oid { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? CreatedDateUtc { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? ModifiedDateUtc { get; set; }

    public Guid? Account { get; set; }

    public Guid? Language { get; set; }

    public string? Parameter { get; set; }

    public string? ParameterValue { get; set; }

    public string? Description { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public virtual Account? AccountNavigation { get; set; }

    public virtual Language? LanguageNavigation { get; set; }
}
