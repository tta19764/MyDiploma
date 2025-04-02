using System;
using System.Collections.Generic;

namespace HumanRecourcesApp.Models;

public partial class PerformanceReview
{
    public int ReviewId { get; set; }

    public int EmployeeId { get; set; }

    public int ReviewerId { get; set; }

    public string? ReviewPeriod { get; set; }

    public DateOnly ReviewDate { get; set; }

    public decimal? OverallRating { get; set; }

    public string? Comments { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? SubmissionDate { get; set; }

    public DateTime? AcknowledgementDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<PerformanceScore> PerformanceScores { get; set; } = new List<PerformanceScore>();

    public virtual Employee Reviewer { get; set; } = null!;
}
