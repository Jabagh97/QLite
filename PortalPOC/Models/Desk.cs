using System;
using System.Collections.Generic;

namespace PortalPOC.Models;

public partial class Desk
{
    public Guid Oid { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? CreatedDateUtc { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? ModifiedDateUtc { get; set; }

    public string? Name { get; set; }

    public Guid? Account { get; set; }

    public Guid? Branch { get; set; }

    public Guid? Pano { get; set; }

    public string? DisplayNo { get; set; }

    public int? ActivityStatus { get; set; }

    public int? CurrentTicketNumber { get; set; }

    public DateTime? LastStateTime { get; set; }

    public bool? Autocall { get; set; }

    public string? ActiveUser { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public virtual Branch? BranchNavigation { get; set; }

    public virtual ICollection<DeskCreatableService> DeskCreatableServices { get; } = new List<DeskCreatableService>();

    public virtual ICollection<DeskMacroSchedule> DeskMacroSchedules { get; } = new List<DeskMacroSchedule>();

    public virtual ICollection<DeskTransferableService> DeskTransferableServices { get; } = new List<DeskTransferableService>();

    public virtual ICollection<KappUser> KappUserDeskNavigations { get; } = new List<KappUser>();

    public virtual ICollection<KappUser> KappUserLastDeskNavigations { get; } = new List<KappUser>();

    public virtual KioskApplication? PanoNavigation { get; set; }

    public virtual ICollection<Ticket> TicketCurrentDeskNavigations { get; } = new List<Ticket>();

    public virtual ICollection<Ticket> TicketDeskNavigations { get; } = new List<Ticket>();

    public virtual ICollection<TicketState> TicketStates { get; } = new List<TicketState>();

    public virtual ICollection<Ticket> TicketToDeskNavigations { get; } = new List<Ticket>();
}
