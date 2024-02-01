using System;
using System.Collections.Generic;

namespace QLite.Data;

public partial class KappUser
{
    public Guid Oid { get; set; }

    public Guid? Account { get; set; }

    public Guid? AuthorizedBranch { get; set; }

    public Guid? Branch { get; set; }

    public Guid? Desk { get; set; }

    public Guid? LastDesk { get; set; }

    public bool? CanChangeMacro { get; set; }

    public virtual Account? AccountNavigation { get; set; }

    public virtual Branch? AuthorizedBranchNavigation { get; set; }

    public virtual Branch? BranchNavigation { get; set; }

    public virtual ICollection<DeskMacroSchedule> DeskMacroSchedules { get; } = new List<DeskMacroSchedule>();

    public virtual Desk? DeskNavigation { get; set; }

    public virtual Desk? LastDeskNavigation { get; set; }

    public virtual PermissionPolicyUser OidNavigation { get; set; } = null!;

    public virtual ICollection<PermissionPolicyUserLoginInfo> PermissionPolicyUserLoginInfos { get; } = new List<PermissionPolicyUserLoginInfo>();
}
