using QLite.Data.CustomAttribute;
using QLite.Data.ViewModels.Language;
using QLite.Data.ViewModels.Macro;
using System;
using System.Collections.Generic;

namespace QLite.Data;
[ModelMapping(typeof(Macro), typeof(MacroViewModel))]

public partial class Macro
{
    public Guid Oid { get; set; }

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? CreatedDateUtc { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? ModifiedDateUtc { get; set; }

    public Guid? Account { get; set; }

    public int? MacroType { get; set; }

    public string? Name { get; set; }

    public int? ToThisDesk { get; set; }

    public int? OptimisticLockField { get; set; }

    public int? Gcrecord { get; set; }

    public int? MaxWaitingTime { get; set; }

    public virtual Account? AccountNavigation { get; set; }

    public virtual ICollection<DeskMacroSchedule> DeskMacroSchedules { get; } = new List<DeskMacroSchedule>();

    public virtual ICollection<MacroRule> MacroRules { get; } = new List<MacroRule>();
}
