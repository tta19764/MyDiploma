using System;
using System.Collections.Generic;

namespace HumanRecourcesApp.Models;

public partial class TimeOffRequest
{
    public int TimeOffRequestId { get; set; }

    public int EmployeeId { get; set; }

    public int TimeOffTypeId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int TotalDays { get; set; }

    public string? Reason { get; set; }

    public string Status { get; set; } = null!;

    public int? ApprovedBy { get; set; }

    public DateTime? ApprovalDate { get; set; }

    public string? Comments { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Employee? ApprovedByNavigation { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual TimeOffType TimeOffType { get; set; } = null!;
}
