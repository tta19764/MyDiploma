using System;
using System.Collections.Generic;

namespace HumanRecourcesApp.Models;

public partial class TimeOffType
{
    public int TimeOffTypeId { get; set; }

    public string TimeOffTypeName { get; set; } = null!;

    public string? Description { get; set; }

    public int? DefaultDays { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<TimeOffBalance> TimeOffBalances { get; set; } = new List<TimeOffBalance>();

    public virtual ICollection<TimeOffRequest> TimeOffRequests { get; set; } = new List<TimeOffRequest>();
}
