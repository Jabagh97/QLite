using QLite.Data.CustomAttribute;
using QLite.Data.ViewModels.Province;
using System;
using System.Collections.Generic;

namespace QLite.Data;
[ModelMapping(typeof(Province), typeof(ProvinceViewModel))]

public partial class Province
{
    public Guid Oid { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? CreatedDateUtc { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? ModifiedDateUtc { get; set; }

    public Guid? Country { get; set; }

    public string? Name { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public virtual ICollection<Branch> Branches { get; } = new List<Branch>();

    public virtual Country? CountryNavigation { get; set; }

    public virtual ICollection<SubProvince> SubProvinces { get; } = new List<SubProvince>();
}
