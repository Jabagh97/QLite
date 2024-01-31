using System;
using System.Collections.Generic;

namespace PortalPOC.Models;

public partial class VDeskStatus
{
    public string? Account { get; set; }

    public string? Branch { get; set; }

    public string? UserName { get; set; }

    public int? DeskActivityStatus { get; set; }

    public int? ProcessCount { get; set; }

    public TimeSpan? TotalTime { get; set; }

    public TimeSpan? AvarageTime { get; set; }

    public int? TicketCount { get; set; }
}
