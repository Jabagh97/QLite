
using QLite.Data.CustomAttribute;
using QLite.Data.Models;
using QLite.Data.Models.Auth;
using QLite.Data.ViewModels.Account;
using System;
using System.Collections.Generic;

namespace QLite.Data;
[ModelMapping(typeof(Account), typeof(AccountViewModel))]

public partial class Account
{
    public Guid Oid { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? CreatedDateUtc { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? ModifiedDateUtc { get; set; }

    public string? Name { get; set; }

    public string? Code { get; set; }

    public string? Mail { get; set; }


    public byte[]? LogoS { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public virtual ICollection<AccountLanguage> AccountLanguages { get; } = new List<AccountLanguage>();

    public virtual ICollection<Branch> Branches { get; } = new List<Branch>();

    public virtual ICollection<KappSetting> KappSettings { get; } = new List<KappSetting>();

   // public virtual ICollection<ApplicationUser> AppUsers { get; } = new List<ApplicationUser>();

    public virtual ICollection<KioskApplicationType> KioskApplicationTypes { get; } = new List<KioskApplicationType>();

    public virtual ICollection<KioskApplication> KioskApplications { get; } = new List<KioskApplication>();

    public virtual ICollection<Macro> Macros { get; } = new List<Macro>();

    public virtual ICollection<QorchSession> QorchSessions { get; } = new List<QorchSession>();

    public virtual ICollection<Resource> Resources { get; } = new List<Resource>();

    public virtual ICollection<Segment> Segments { get; } = new List<Segment>();

    public virtual ICollection<ServiceType> ServiceTypes { get; } = new List<ServiceType>();

    public virtual ICollection<TicketPoolProfile> TicketPoolProfiles { get; } = new List<TicketPoolProfile>();

    public virtual ICollection<TicketPool> TicketPools { get; } = new List<TicketPool>();
}
