using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

using QLite.Data.CustomAttribute;
using QLite.Data.ViewModels.Country;

namespace QLite.Data;
[ModelMapping(typeof(Country), typeof(CountryViewModel))]

public partial class Country
{
    public Guid Oid { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? CreatedDateUtc { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? ModifiedDateUtc { get; set; }
    [Required]
    [DisplayName("Name")]
    public string? Name { get; set; }

    public string? Mask { get; set; }

    public int? Sequence { get; set; }

    public string? PhoneCode { get; set; }

    public string? LangCode { get; set; }

    public byte[]? Logo { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public virtual ICollection<Branch> Branches { get; } = new List<Branch>();

    public virtual ICollection<Province> Provinces { get; } = new List<Province>();

    public virtual ICollection<SubProvince> SubProvinces { get; } = new List<SubProvince>();
}
