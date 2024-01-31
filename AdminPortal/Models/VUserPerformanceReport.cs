using System;
using System.Collections.Generic;

namespace PortalPOC.Models;

public partial class VUserPerformanceReport
{
    public Guid Oid { get; set; }

    public string? UserName { get; set; }

    public int? TicketCount { get; set; }

    public int TicketCountPercentage { get; set; }

    public TimeSpan? MinProcessTime { get; set; }

    public TimeSpan? MaxProcessTime { get; set; }

    public TimeSpan? AvgProcessTime { get; set; }

    public DateTime? TotalProcessTime { get; set; }

    public int TotalProcessTimePercentage { get; set; }

    public TimeSpan? MinWaitingTime { get; set; }

    public TimeSpan? MaxWaitingTime { get; set; }

    public TimeSpan? AvgWaitingTime { get; set; }

    public DateTime? TotalWaitingTime { get; set; }

    public int TotalWaitingTimePercentage { get; set; }
}
