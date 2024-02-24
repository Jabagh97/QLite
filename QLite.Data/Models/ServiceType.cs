using NPoco;
using QLite.Data.CustomAttribute;
using QLite.Data.ViewModels.Resource;
using QLite.Data.ViewModels.ServiceType;
using System;
using System.Collections.Generic;

namespace QLite.Data;
[ModelMapping(typeof(ServiceType), typeof(ServiceTypeViewModel))]

public partial class ServiceType
{
    public Guid Oid { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? CreatedDateUtc { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? ModifiedDateUtc { get; set; }

    public Guid? Account { get; set; }

    public string? Key { get; set; }

    public string? Name { get; set; }

    public byte[]? Icon { get; set; }

    public Guid? Parent { get; set; }

    public bool? IsParent { get; set; }

    public bool? CallInKiosk { get; set; }

    public bool? GenTicketByDesk { get; set; }

    public bool? Default { get; set; }

    public int? SeqNo { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public virtual Account? AccountNavigation { get; set; }



    public virtual ICollection<DeskCreatableService> DeskCreatableServices { get; } = new List<DeskCreatableService>();

    public virtual ICollection<DeskTransferableService> DeskTransferableServices { get; } = new List<DeskTransferableService>();

    public virtual ICollection<ServiceType> InverseParentNavigation { get; } = new List<ServiceType>();

    public virtual ICollection<MacroRule> MacroRules { get; } = new List<MacroRule>();

    public virtual ServiceType? ParentNavigation { get; set; }

    public virtual ICollection<QorchSession> QorchSessions { get; } = new List<QorchSession>();

    public virtual ICollection<TicketPool> TicketPools { get; } = new List<TicketPool>();

    public virtual ICollection<Ticket> TicketServiceTypeNavigations { get; } = new List<Ticket>();

    public virtual ICollection<TicketState> TicketStates { get; } = new List<TicketState>();

    public virtual ICollection<Ticket> TicketToServiceTypeNavigations { get; } = new List<Ticket>();


}
