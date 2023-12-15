using System;
using System.Collections.Generic;

namespace PortalPOC.Models;

public partial class KioskApplication
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

    public Guid? KioskApplicationType { get; set; }

    public Guid? KappWorkflow { get; set; }

    public string? KappName { get; set; }

    public string? HwId { get; set; }

    public string? DesignTag { get; set; }

    public string? PlatformAuthClientId { get; set; }

    public string? PlatformAuthClientSecret { get; set; }

    public bool? Active { get; set; }

    public bool? HasDigitalDisplay { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public virtual Account? AccountNavigation { get; set; }

    public virtual Branch? BranchNavigation { get; set; }

    public virtual ICollection<Desk> Desks { get; } = new List<Desk>();

    public virtual ICollection<KappRelation> KappRelationChildNavigations { get; } = new List<KappRelation>();

    public virtual ICollection<KappRelation> KappRelationParentNavigations { get; } = new List<KappRelation>();

    public virtual ICollection<KappSetting> KappSettings { get; } = new List<KappSetting>();

    public virtual KappWorkflow? KappWorkflowNavigation { get; set; }

    public virtual KioskApplicationType? KioskApplicationTypeNavigation { get; set; }

    public virtual ICollection<QorchSession> QorchSessions { get; } = new List<QorchSession>();

    public virtual ICollection<TicketPool> TicketPools { get; } = new List<TicketPool>();
}
