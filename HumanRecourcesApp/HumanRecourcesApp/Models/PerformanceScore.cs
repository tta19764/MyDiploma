using System;
using System.Collections.Generic;

namespace HumanRecourcesApp.Models;

public partial class PerformanceScore
{
    public int ScoreId { get; set; }

    public int ReviewId { get; set; }

    public int CriteriaId { get; set; }

    public decimal Score { get; set; }

    public string? Comments { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual PerformanceCriterion Criteria { get; set; } = null!;

    public virtual PerformanceReview Review { get; set; } = null!;
}
