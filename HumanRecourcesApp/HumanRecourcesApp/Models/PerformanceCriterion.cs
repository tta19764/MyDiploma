using System;
using System.Collections.Generic;

namespace HumanRecourcesApp.Models;

public partial class PerformanceCriterion
{
    public int CriteriaId { get; set; }

    public string CriteriaName { get; set; } = null!;

    public string? Description { get; set; }

    public string? Category { get; set; }

    public decimal? WeightPercentage { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<PerformanceScore> PerformanceScores { get; set; } = new List<PerformanceScore>();
}
