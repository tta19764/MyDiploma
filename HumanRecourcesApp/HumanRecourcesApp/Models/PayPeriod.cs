using System;
using System.Collections.Generic;

namespace HumanResourcesApp.Models;

public partial class PayPeriod
{
    public int PayPeriodId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public DateOnly PaymentDate { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<EmployeePayroll> EmployeePayrolls { get; set; } = new List<EmployeePayroll>();
}
