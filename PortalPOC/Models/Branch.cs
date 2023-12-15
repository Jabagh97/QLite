using System;
using System.Collections.Generic;

namespace PortalPOC.Models;

public partial class Branch
{
    public Guid Oid { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? CreatedDateUtc { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? ModifiedDateUtc { get; set; }

    public Guid? Account { get; set; }

    public Guid? Country { get; set; }

    public Guid? Province { get; set; }

    public Guid? SubProvince { get; set; }

    public string? Name { get; set; }

    public string? BranchCode { get; set; }

    public string? Terminal { get; set; }

    public string? Area { get; set; }

    public Guid? KioskRestartProfile { get; set; }

    public string? Address { get; set; }

    public string? Address2 { get; set; }

    public Guid? TicketPoolProfile { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public virtual Account? AccountNavigation { get; set; }

    public virtual ICollection<AppointmentSetting> AppointmentSettings { get; } = new List<AppointmentSetting>();

    public virtual ICollection<Appointment> Appointments { get; } = new List<Appointment>();

    public virtual Country? CountryNavigation { get; set; }

    public virtual ICollection<DeskCreatableService> DeskCreatableServices { get; } = new List<DeskCreatableService>();

    public virtual ICollection<DeskMacroSchedule> DeskMacroSchedules { get; } = new List<DeskMacroSchedule>();

    public virtual ICollection<DeskTransferableService> DeskTransferableServices { get; } = new List<DeskTransferableService>();

    public virtual ICollection<Desk> Desks { get; } = new List<Desk>();

    public virtual ICollection<KappSetting> KappSettings { get; } = new List<KappSetting>();

    public virtual ICollection<KappUser> KappUserAuthorizedBranchNavigations { get; } = new List<KappUser>();

    public virtual ICollection<KappUser> KappUserBranchNavigations { get; } = new List<KappUser>();

    public virtual ICollection<KioskApplication> KioskApplications { get; } = new List<KioskApplication>();

    public virtual Province? ProvinceNavigation { get; set; }

    public virtual SubProvince? SubProvinceNavigation { get; set; }

    public virtual TicketPoolProfile? TicketPoolProfileNavigation { get; set; }

    public virtual ICollection<TicketPool> TicketPools { get; } = new List<TicketPool>();

    public virtual ICollection<TicketState> TicketStates { get; } = new List<TicketState>();

    public virtual ICollection<Ticket> Tickets { get; } = new List<Ticket>();
}
