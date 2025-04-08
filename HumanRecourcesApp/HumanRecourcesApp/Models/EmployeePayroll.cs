using System;
using System.Collections.Generic;

namespace HumanResourcesApp.Models;

public partial class EmployeePayroll
{
    public int PayrollId { get; set; }

    public int EmployeeId { get; set; }

    public int PayPeriodId { get; set; }

    public decimal BaseSalary { get; set; }

    public decimal GrossSalary { get; set; }

    public decimal TotalDeductions { get; set; }

    public decimal NetSalary { get; set; }

    public string? Status { get; set; }

    public int? ProcessedBy { get; set; }

    public DateTime? ProcessedDate { get; set; }

    public string? PaymentReference { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual PayPeriod PayPeriod { get; set; } = null!;

    public virtual ICollection<PayrollDetail> PayrollDetails { get; set; } = new List<PayrollDetail>();

    public virtual User? ProcessedByNavigation { get; set; }
}
