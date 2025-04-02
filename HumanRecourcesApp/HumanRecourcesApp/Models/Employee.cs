using System;
using System.Collections.Generic;

namespace HumanRecourcesApp.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public int? UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public char? Gender { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public DateOnly HireDate { get; set; }

    public DateOnly? TerminationDate { get; set; }

    public int? PositionId { get; set; }

    public decimal? Salary { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual ICollection<EmployeePayroll> EmployeePayrolls { get; set; } = new List<EmployeePayroll>();

    public virtual ICollection<PerformanceReview> PerformanceReviewEmployees { get; set; } = new List<PerformanceReview>();

    public virtual ICollection<PerformanceReview> PerformanceReviewReviewers { get; set; } = new List<PerformanceReview>();

    public virtual Position? Position { get; set; }

    public virtual ICollection<TimeOffBalance> TimeOffBalances { get; set; } = new List<TimeOffBalance>();

    public virtual ICollection<TimeOffRequest> TimeOffRequestApprovedByNavigations { get; set; } = new List<TimeOffRequest>();

    public virtual ICollection<TimeOffRequest> TimeOffRequestEmployees { get; set; } = new List<TimeOffRequest>();

    public virtual User? User { get; set; }
}
