using System;
using System.Collections.Generic;

namespace HumanResourcesApp.Models;

public partial class PayrollItem
{
    public int PayrollItemId { get; set; }

    public string ItemName { get; set; } = null!;

    public string ItemType { get; set; } = null!;

    public bool? IsPercentageBased { get; set; }

    public decimal? DefaultValue { get; set; }

    public bool? TaxableFlag { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<PayrollDetail> PayrollDetails { get; set; } = new List<PayrollDetail>();
}
