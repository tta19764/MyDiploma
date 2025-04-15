using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.EntityFrameworkCore;

namespace HumanResourcesApp.Models;

public partial class HumanResourcesDbContext : DbContext
{
    public HumanResourcesDbContext()
    {
    }

    public HumanResourcesDbContext(DbContextOptions<HumanResourcesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeePayroll> EmployeePayrolls { get; set; }

    public virtual DbSet<PayPeriod> PayPeriods { get; set; }

    public virtual DbSet<PayrollDetail> PayrollDetails { get; set; }

    public virtual DbSet<PayrollItem> PayrollItems { get; set; }

    public virtual DbSet<PerformanceCriterion> PerformanceCriteria { get; set; }

    public virtual DbSet<PerformanceReview> PerformanceReviews { get; set; }

    public virtual DbSet<PerformanceScore> PerformanceScores { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<SystemLog> SystemLogs { get; set; }

    public virtual DbSet<TimeOffBalance> TimeOffBalances { get; set; }

    public virtual DbSet<TimeOffRequest> TimeOffRequests { get; set; }

    public virtual DbSet<TimeOffType> TimeOffTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["HumanResourcesDb"].ConnectionString;

            if(string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'HumanResourcesDb' not found.");
            }

            optionsBuilder
                .UseLazyLoadingProxies()
                .UseNpgsql(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.AttendanceId).HasName("attendance_pkey");

            entity.ToTable("attendance");

            entity.HasIndex(e => new { e.CheckInTime, e.CheckOutTime }, "idx_attendance_dates");

            entity.HasIndex(e => e.EmployeeId, "idx_attendance_employee");

            entity.Property(e => e.AttendanceId).HasColumnName("attendance_id");
            entity.Property(e => e.CheckInTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("check_in_time");
            entity.Property(e => e.CheckOutTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("check_out_time");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.WorkHours)
                .HasPrecision(5, 2)
                .HasColumnName("work_hours");

            entity.HasOne(d => d.Employee).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("attendance_employee_id_fkey");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("departments_pkey");

            entity.ToTable("departments");

            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(100)
                .HasColumnName("department_name");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.ManagerId).HasColumnName("manager_id");

            entity.HasOne(d => d.Manager).WithMany(p => p.Departments)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("fk_department_manager");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("employees_pkey");

            entity.ToTable("employees");

            entity.HasIndex(e => e.Email, "employees_email_key").IsUnique();

            entity.HasIndex(e => e.UserId, "employees_user_id_key").IsUnique();

            entity.HasIndex(e => e.PositionId, "idx_employees_position");

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .HasColumnName("country");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .HasColumnName("gender");
            entity.Property(e => e.HireDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("hire_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .HasColumnName("middle_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.PositionId).HasColumnName("position_id");
            entity.Property(e => e.Salary)
                .HasPrecision(10, 2)
                .HasColumnName("salary");
            entity.Property(e => e.TerminationDate).HasColumnName("termination_date");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Department).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employees_department_id_fkey");

            entity.HasOne(d => d.Position).WithMany(p => p.Employees)
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employees_position_id_fkey");

            entity.HasOne(d => d.User).WithOne(p => p.Employee)
                .HasForeignKey<Employee>(d => d.UserId)
                .HasConstraintName("employees_user_id_fkey");
        });

        modelBuilder.Entity<EmployeePayroll>(entity =>
        {
            entity.HasKey(e => e.PayrollId).HasName("employee_payroll_pkey");

            entity.ToTable("employee_payroll");

            entity.HasIndex(e => new { e.EmployeeId, e.PayPeriodId }, "employee_payroll_employee_id_pay_period_id_key").IsUnique();

            entity.HasIndex(e => e.EmployeeId, "idx_payroll_employee");

            entity.HasIndex(e => e.PayPeriodId, "idx_payroll_period");

            entity.Property(e => e.PayrollId).HasColumnName("payroll_id");
            entity.Property(e => e.BaseSalary)
                .HasPrecision(10, 2)
                .HasColumnName("base_salary");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.GrossSalary)
                .HasPrecision(10, 2)
                .HasColumnName("gross_salary");
            entity.Property(e => e.NetSalary)
                .HasPrecision(10, 2)
                .HasColumnName("net_salary");
            entity.Property(e => e.PayPeriodId).HasColumnName("pay_period_id");
            entity.Property(e => e.PaymentReference)
                .HasMaxLength(100)
                .HasColumnName("payment_reference");
            entity.Property(e => e.ProcessedBy).HasColumnName("processed_by");
            entity.Property(e => e.ProcessedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("processed_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TotalDeductions)
                .HasPrecision(10, 2)
                .HasColumnName("total_deductions");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeePayrolls)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employee_payroll_employee_id_fkey");

            entity.HasOne(d => d.PayPeriod).WithMany(p => p.EmployeePayrolls)
                .HasForeignKey(d => d.PayPeriodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employee_payroll_pay_period_id_fkey");

            entity.HasOne(d => d.ProcessedByNavigation).WithMany(p => p.EmployeePayrolls)
                .HasForeignKey(d => d.ProcessedBy)
                .HasConstraintName("employee_payroll_processed_by_fkey");
        });

        modelBuilder.Entity<PayPeriod>(entity =>
        {
            entity.HasKey(e => e.PayPeriodId).HasName("pay_periods_pkey");

            entity.ToTable("pay_periods");

            entity.Property(e => e.PayPeriodId).HasColumnName("pay_period_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.PaymentDate).HasColumnName("payment_date");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Open'::character varying")
                .HasColumnName("status");
        });

        modelBuilder.Entity<PayrollDetail>(entity =>
        {
            entity.HasKey(e => e.PayrollDetailId).HasName("payroll_details_pkey");

            entity.ToTable("payroll_details");

            entity.Property(e => e.PayrollDetailId).HasColumnName("payroll_detail_id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.PayrollId).HasColumnName("payroll_id");
            entity.Property(e => e.PayrollItemId).HasColumnName("payroll_item_id");

            entity.HasOne(d => d.Payroll).WithMany(p => p.PayrollDetails)
                .HasForeignKey(d => d.PayrollId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("payroll_details_payroll_id_fkey");

            entity.HasOne(d => d.PayrollItem).WithMany(p => p.PayrollDetails)
                .HasForeignKey(d => d.PayrollItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("payroll_details_payroll_item_id_fkey");
        });

        modelBuilder.Entity<PayrollItem>(entity =>
        {
            entity.HasKey(e => e.PayrollItemId).HasName("payroll_items_pkey");

            entity.ToTable("payroll_items");

            entity.Property(e => e.PayrollItemId).HasColumnName("payroll_item_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DefaultValue)
                .HasPrecision(10, 2)
                .HasColumnName("default_value");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsPercentageBased)
                .HasDefaultValue(false)
                .HasColumnName("is_percentage_based");
            entity.Property(e => e.ItemName)
                .HasMaxLength(100)
                .HasColumnName("item_name");
            entity.Property(e => e.ItemType)
                .HasMaxLength(20)
                .HasColumnName("item_type");
            entity.Property(e => e.TaxableFlag)
                .HasDefaultValue(true)
                .HasColumnName("taxable_flag");
        });

        modelBuilder.Entity<PerformanceCriterion>(entity =>
        {
            entity.HasKey(e => e.CriteriaId).HasName("performance_criteria_pkey");

            entity.ToTable("performance_criteria");

            entity.Property(e => e.CriteriaId).HasColumnName("criteria_id");
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .HasColumnName("category");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CriteriaName)
                .HasMaxLength(100)
                .HasColumnName("criteria_name");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.WeightPercentage)
                .HasPrecision(5, 2)
                .HasColumnName("weight_percentage");
        });

        modelBuilder.Entity<PerformanceReview>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("performance_reviews_pkey");

            entity.ToTable("performance_reviews");

            entity.HasIndex(e => e.EmployeeId, "idx_performance_reviews_employee");

            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.AcknowledgementDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("acknowledgement_date");
            entity.Property(e => e.Comments).HasColumnName("comments");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.OverallRating)
                .HasPrecision(3, 2)
                .HasColumnName("overall_rating");
            entity.Property(e => e.ReviewDate).HasColumnName("review_date");
            entity.Property(e => e.ReviewPeriod)
                .HasMaxLength(50)
                .HasColumnName("review_period");
            entity.Property(e => e.ReviewerId).HasColumnName("reviewer_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Draft'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.SubmissionDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("submission_date");

            entity.HasOne(d => d.Employee).WithMany(p => p.PerformanceReviewEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("performance_reviews_employee_id_fkey");

            entity.HasOne(d => d.Reviewer).WithMany(p => p.PerformanceReviewReviewers)
                .HasForeignKey(d => d.ReviewerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("performance_reviews_reviewer_id_fkey");
        });

        modelBuilder.Entity<PerformanceScore>(entity =>
        {
            entity.HasKey(e => e.ScoreId).HasName("performance_scores_pkey");

            entity.ToTable("performance_scores");

            entity.HasIndex(e => new { e.ReviewId, e.CriteriaId }, "performance_scores_review_id_criteria_id_key").IsUnique();

            entity.Property(e => e.ScoreId).HasColumnName("score_id");
            entity.Property(e => e.Comments).HasColumnName("comments");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CriteriaId).HasColumnName("criteria_id");
            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.Score)
                .HasPrecision(3, 2)
                .HasColumnName("score");

            entity.HasOne(d => d.Criteria).WithMany(p => p.PerformanceScores)
                .HasForeignKey(d => d.CriteriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("performance_scores_criteria_id_fkey");

            entity.HasOne(d => d.Review).WithMany(p => p.PerformanceScores)
                .HasForeignKey(d => d.ReviewId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("performance_scores_review_id_fkey");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("permissions_pkey");

            entity.ToTable("permissions");

            entity.HasIndex(e => e.PermissionName, "permissions_permission_name_key").IsUnique();

            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.PermissionName)
                .HasMaxLength(50)
                .HasColumnName("permission_name");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.PositionId).HasName("positions_pkey");

            entity.ToTable("positions");

            entity.Property(e => e.PositionId).HasColumnName("position_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.PositionTitle)
                .HasMaxLength(100)
                .HasColumnName("position_title");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.HasIndex(e => e.RoleName, "roles_role_name_key").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.RoleName)
                .HasMaxLength(30)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.PermissionId }).HasName("role_permissions_pkey");

            entity.ToTable("role_permissions");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("role_permissions_permission_id_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("role_permissions_role_id_fkey");
        });

        modelBuilder.Entity<SystemLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("system_logs_pkey");

            entity.ToTable("system_logs");

            entity.HasIndex(e => e.Action, "idx_system_logs_action");

            entity.HasIndex(e => e.LogDate, "idx_system_logs_date");

            entity.HasIndex(e => e.LogLevel, "idx_system_logs_level");

            entity.HasIndex(e => e.UserId, "idx_system_logs_user");

            entity.Property(e => e.LogId).HasColumnName("log_id");
            entity.Property(e => e.Action)
                .HasMaxLength(100)
                .HasColumnName("action");
            entity.Property(e => e.AdditionalInfo).HasColumnName("additional_info");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.EntityType)
                .HasMaxLength(50)
                .HasColumnName("entity_type");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(50)
                .HasColumnName("ip_address");
            entity.Property(e => e.LogDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("log_date");
            entity.Property(e => e.LogLevel)
                .HasMaxLength(20)
                .HasColumnName("log_level");
            entity.Property(e => e.LogSource)
                .HasMaxLength(100)
                .HasColumnName("log_source");
            entity.Property(e => e.NewValues)
                .HasColumnType("jsonb")
                .HasColumnName("new_values");
            entity.Property(e => e.OldValues)
                .HasColumnType("jsonb")
                .HasColumnName("old_values");
            entity.Property(e => e.UserAgent)
                .HasMaxLength(255)
                .HasColumnName("user_agent");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.SystemLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("system_logs_user_id_fkey");
        });

        modelBuilder.Entity<TimeOffBalance>(entity =>
        {
            entity.HasKey(e => e.TimeOffBalanceId).HasName("time_off_balances_pkey");

            entity.ToTable("time_off_balances");

            entity.HasIndex(e => new { e.EmployeeId, e.TimeOffTypeId, e.Period }, "time_off_balances_employee_id_time_off_type_id_period_key").IsUnique();

            entity.Property(e => e.TimeOffBalanceId).HasColumnName("time_off_balance_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.Period)
                .HasMaxLength(10)
                .HasColumnName("period");
            entity.Property(e => e.RemainingDays).HasColumnName("remaining_days");
            entity.Property(e => e.TimeOffTypeId).HasColumnName("time_off_type_id");
            entity.Property(e => e.TotalDays).HasColumnName("total_days");
            entity.Property(e => e.UsedDays)
                .HasDefaultValue(0)
                .HasColumnName("used_days");

            entity.HasOne(d => d.Employee).WithMany(p => p.TimeOffBalances)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("time_off_balances_employee_id_fkey");

            entity.HasOne(d => d.TimeOffType).WithMany(p => p.TimeOffBalances)
                .HasForeignKey(d => d.TimeOffTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("time_off_balances_time_off_type_id_fkey");
        });

        modelBuilder.Entity<TimeOffRequest>(entity =>
        {
            entity.HasKey(e => e.TimeOffRequestId).HasName("time_off_requests_pkey");

            entity.ToTable("time_off_requests");

            entity.HasIndex(e => new { e.StartDate, e.EndDate }, "idx_time_off_requests_dates");

            entity.HasIndex(e => e.EmployeeId, "idx_time_off_requests_employee");

            entity.HasIndex(e => e.Status, "idx_time_off_requests_status");

            entity.Property(e => e.TimeOffRequestId).HasColumnName("time_off_request_id");
            entity.Property(e => e.ApprovalDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("approval_date");
            entity.Property(e => e.ApprovedBy).HasColumnName("approved_by");
            entity.Property(e => e.Comments).HasColumnName("comments");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TimeOffTypeId).HasColumnName("time_off_type_id");
            entity.Property(e => e.TotalDays).HasColumnName("total_days");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.TimeOffRequestApprovedByNavigations)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("time_off_requests_approved_by_fkey");

            entity.HasOne(d => d.Employee).WithMany(p => p.TimeOffRequestEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("time_off_requests_employee_id_fkey");

            entity.HasOne(d => d.TimeOffType).WithMany(p => p.TimeOffRequests)
                .HasForeignKey(d => d.TimeOffTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("time_off_requests_time_off_type_id_fkey");
        });

        modelBuilder.Entity<TimeOffType>(entity =>
        {
            entity.HasKey(e => e.TimeOffTypeId).HasName("time_off_types_pkey");

            entity.ToTable("time_off_types");

            entity.HasIndex(e => e.TimeOffTypeName, "time_off_types_time_off_type_name_key").IsUnique();

            entity.Property(e => e.TimeOffTypeId).HasColumnName("time_off_type_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DefaultDays).HasColumnName("default_days");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.TimeOffTypeName)
                .HasMaxLength(50)
                .HasColumnName("time_off_type_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LastLogin)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_login");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_users_roles_role_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
