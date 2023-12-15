using System;
using System.Collections.Generic;

namespace PortalPOC.Models;

public partial class Language
{
    public Guid Oid { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? CreatedDateUtc { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? ModifiedDateUtc { get; set; }

    public string? Name { get; set; }

    public string? EnglishName { get; set; }

    public string? LocalName { get; set; }

    public string? CultureInfo { get; set; }

    public string? LangCode { get; set; }

    public byte[]? Logo { get; set; }

    public bool? IsDefault { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public virtual ICollection<AccountLanguage> AccountLanguages { get; } = new List<AccountLanguage>();

    public virtual ICollection<Resource> Resources { get; } = new List<Resource>();
}
