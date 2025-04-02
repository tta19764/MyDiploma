using System;
using System.Collections.Generic;

namespace HumanRecourcesApp.Models;

public partial class PayrollDetail
{
    public int PayrollDetailId { get; set; }

    public int PayrollId { get; set; }

    public int PayrollItemId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual EmployeePayroll Payroll { get; set; } = null!;

    public virtual PayrollItem PayrollItem { get; set; } = null!;
}
