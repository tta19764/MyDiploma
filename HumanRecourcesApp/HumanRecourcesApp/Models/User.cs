﻿using System;
using System.Collections.Generic;

namespace HumanResourcesApp.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? LastLogin { get; set; }

    public int RoleId { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<EmployeePayroll> EmployeePayrolls { get; set; } = new List<EmployeePayroll>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<SystemLog> SystemLogs { get; set; } = new List<SystemLog>();
}
