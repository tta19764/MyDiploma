using System;
using System.Collections.Generic;

namespace HumanRecourcesApp.Models;

public partial class TimeOffBalance
{
    public int TimeOffBalanceId { get; set; }

    public int EmployeeId { get; set; }

    public int TimeOffTypeId { get; set; }

    public string Period { get; set; } = null!;

    public int TotalDays { get; set; }

    public int? UsedDays { get; set; }

    public int? RemainingDays { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual TimeOffType TimeOffType { get; set; } = null!;
}
