using System;
using System.Collections.Generic;

namespace HumanResourcesApp.Models;

public partial class SystemLog
{
    public int LogId { get; set; }

    public int? UserId { get; set; }

    public DateTime? LogDate { get; set; }

    public string LogLevel { get; set; } = null!;

    public string LogSource { get; set; } = null!;

    public string Action { get; set; } = null!;

    public string? EntityType { get; set; }

    public int? EntityId { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public string? AdditionalInfo { get; set; }

    public virtual User? User { get; set; }
}
