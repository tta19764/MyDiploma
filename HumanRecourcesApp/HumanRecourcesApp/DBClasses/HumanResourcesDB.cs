using HumanResourcesApp.Classes;
using HumanResourcesApp.Models;
using HumanResourcesApp.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace HumanResourcesApp.DBClasses
{
    internal class HumanResourcesDB
    {
        private HumanResourcesDbContext _context;
        public HumanResourcesDB()
        {
            _context = new HumanResourcesDbContext();
        }

        private bool HasPermission(User user, string permissionName)
        {
            return user.Role.RolePermissions.Any(rp => rp.Permission.PermissionName == permissionName);
        }

        private object TrimForLog(object entity)
        {
            // Create a shallow copy and null out navigation properties to avoid cycles
            var type = entity.GetType();
            var clone = Activator.CreateInstance(type);

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead || !prop.CanWrite) continue;

                var value = prop.GetValue(entity);

                // Exclude navigation properties (collections or other entities)
                if (prop.PropertyType.Namespace?.StartsWith("HumanResourcesManagement.Models") == true && prop.PropertyType != typeof(string))
                    continue;

                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
                    continue;

                prop.SetValue(clone, value);
            }

            return clone!;
        }

        private void LogOperation(User user, string action, string entityType, int? entityId, object? oldValues, object? newValues)
        {
            try
            {
                var log = new SystemLog
                {
                    UserId = user.UserId,
                    LogDate = DateTime.Now,
                    LogLevel = "Info",
                    LogSource = "HumanResourcesDB",
                    Action = action,
                    EntityType = entityType,
                    EntityId = entityId,
                    OldValues = oldValues != null ? JsonSerializer.Serialize(TrimForLog(oldValues)) : null,
                    NewValues = newValues != null ? JsonSerializer.Serialize(TrimForLog(newValues)) : null,
                    IpAddress = IpAddress.GetLocalIpAddress(),
                    UserAgent = Environment.MachineName
                };

                _context.SystemLogs.Add(log);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                // You can optionally log this to a file or show a message in development
                //MessageBox.Show($"Error logging operation: {ex.Message}", "Logging Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LogError(User user, string sourceMethod, Exception ex)
        {
            try
            {
                var log = new SystemLog
                {
                    UserId = user.UserId,
                    LogDate = DateTime.Now,
                    LogLevel = "Error",
                    LogSource = $"HRMS::{sourceMethod}",
                    Action = "Exception",
                    EntityType = null,
                    EntityId = null,
                    OldValues = null,
                    NewValues = ex.ToString(),
                    IpAddress = IpAddress.GetLocalIpAddress(),
                    UserAgent = Environment.MachineName
                };
                _context.SystemLogs.Add(log);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                // Fallback if logging fails
                //MessageBox.Show($"Error logging operation: {ex.Message}", "Logging Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // =====================================
        // READ OPERATIONS
        // =====================================

        // User Read Operations
        public List<User> GetAllUsers()
        {
            return _context.Users.AsNoTracking().ToList();
        }

        public User? GetUserByLogin(string username, string password)
        {
            var user = _context.Users.AsNoTracking().FirstOrDefault(u => u.Username == username);

            if (user != null && Login.VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }
            else
            {
                return null;
            }
        }

        public Role? GetRoleById(int id)
        {
            return _context.Roles.AsNoTracking().FirstOrDefault(s => s.RoleId == id);
        }

        public User? GetUserById(int id)
        {
            return _context.Users.AsNoTracking().FirstOrDefault(s => s.UserId == id);
        }

        public bool IsUniqueLogin(User user)
        {
            if (_context.Users.AsNoTracking().Count() == 0)
            {
                return true;
            }
            else if (user.UserId == 0)
            {
                return !_context.Users.AsNoTracking().Any(d => d.Username.ToLower() == user.Username.Trim().ToLower());
            }
            else
            {
                return !_context.Users.AsNoTracking().Any(d => d.Username.ToLower() == user.Username.Trim().ToLower() && d.UserId != user.UserId);
            }
        }

        // Department Read Operations
        public List<Department> GetAllDepartments()
        {
            return _context.Departments.AsNoTracking().ToList();
        }

        public Department? GetDepartmentById(int id)
        {
            return _context.Departments.AsNoTracking().FirstOrDefault(s => s.DepartmentId == id);
        }

        public bool IsUniqueDepartmentName(Department department)
        {
            if (_context.Departments.AsNoTracking().Count() == 0)
            {
                return true;
            }
            else if (department.DepartmentId == 0)
            {
                return !_context.Departments.AsNoTracking().Any(d => d.DepartmentName.ToLower() == department.DepartmentName.Trim().ToLower());
            }
            else
            {
                return !_context.Departments.AsNoTracking().Any(d => d.DepartmentName.ToLower() == department.DepartmentName.Trim().ToLower() && d.DepartmentId != department.DepartmentId);
            }
        }

        // Employee Read Operations
        public List<Employee> GetAllEmplyees()
        {
            return _context.Employees.AsNoTracking().ToList();
        }

        public Employee? GetEmployeeById(int id)
        {
            return _context.Employees.AsNoTracking().FirstOrDefault(s => s.EmployeeId == id);
        }

        public List<Employee> GetAllEmplyeesByPosition(Position position)
        {
            return _context.Employees.AsNoTracking().Where(s => s.Position == position).ToList();
        }

        public List<Employee> GetAllFreeEmplyeesByPosition(Position position)
        {
            return _context.Employees.AsNoTracking().Where(s => s.Position == position && s.Departments.Count() == 0).ToList();
        }

        public List<Employee> GetEmployeesByDepartment(Department department)
        {
            return _context.Employees.AsNoTracking().Where(s => s.Department == department).ToList();
        }

        public List<Employee> GetAllActiveEmployees()
        {
            return _context.Employees.AsNoTracking().Where(s => s.IsActive == true).ToList();
        }

        public bool CanDeleteEmployee(int employeeId)
        {
            // Checks that would prevent deletion
            var isManager = _context.Departments.Any(d => d.ManagerId == employeeId);
            if (isManager)
                return false; // Can't delete a department manager

            var isPerformanceReviewer = _context.PerformanceReviews
                .Any(r => r.ReviewerId == employeeId);
            if (isPerformanceReviewer)
                return false;

            var isTimeOffApprover = _context.TimeOffRequests
                .Any(r => r.ApprovedBy == employeeId);
            if (isTimeOffApprover)
                return false;

            // If all checks pass, employee can be deleted
            return true;
        }

        // Position Read Operations
        public List<Position> GetAllPositions()
        {
            return _context.Positions.AsNoTracking().ToList();
        }

        public Position? GetPositionById(int id)
        {
            return _context.Positions.AsNoTracking().FirstOrDefault(s => s.PositionId == id);
        }

        public bool IsUniquePositionTitle(Position position)
        {
            if (_context.Positions.AsNoTracking().Count() == 0)
            {
                return true;
            }
            else if (position.PositionId == 0)
            {
                return !_context.Positions.AsNoTracking().Any(d => d.PositionTitle.ToLower() == position.PositionTitle.Trim().ToLower());
            }
            else
            {
                return !_context.Positions.AsNoTracking().Any(d => d.PositionTitle.ToLower() == position.PositionTitle.Trim().ToLower() && d.PositionId != position.PositionId);
            }
        }

        public bool IsPositionUsedInEmployees(int positionId)
        {
            return _context.Employees.AsNoTracking().Any(d => d.PositionId == positionId);
        }

        // Attendance Read Operations
        public List<Attendance> GetAllAttendances()
        {
            return _context.Attendances.AsNoTracking().ToList();
        }

        public Attendance? GetAttendanceById(int id)
        {
            return _context.Attendances.AsNoTracking().FirstOrDefault(s => s.AttendanceId == id);
        }

        public List<Attendance> GetAttendancesByEmployeeId(int employeeId)
        {
            return _context.Attendances.AsNoTracking().Where(s => s.EmployeeId == employeeId).ToList();
        }

        public bool IsAttendanceTimeValid(Attendance attendance)
        {
            // Return false if either time is null, as we can't perform the check
            if (!attendance.CheckInTime.HasValue || !attendance.CheckOutTime.HasValue)
                return false;

            // Ensure check-out time is after check-in time
            if (attendance.CheckOutTime <= attendance.CheckInTime)
                return false;

            // Query for any overlapping attendance records for this employee
            var overlappingAttendances = _context.Attendances
                .AsNoTracking()
                .Where(a => a.EmployeeId == attendance.EmployeeId)
                // Exclude the current attendance record if we're updating an existing one
                .Where(a =>
                    (attendance.CheckInTime >= a.CheckInTime && attendance.CheckInTime < a.CheckOutTime) ||
                    (attendance.CheckOutTime > a.CheckInTime && attendance.CheckOutTime <= a.CheckOutTime) ||
                    (attendance.CheckInTime <= a.CheckInTime && attendance.CheckOutTime >= a.CheckOutTime) ||
                    (attendance.CheckInTime >= a.CheckInTime && attendance.CheckOutTime <= a.CheckOutTime))
                .Any();

            // Return true if no overlaps found
            return !overlappingAttendances;
        }

        // TimeOffTypes Read Operations

        public List<TimeOffType> GetAllTimeOffTypes()
        {
            return _context.TimeOffTypes.AsNoTracking().ToList();
        }

        public TimeOffType? GetTimeOffTypeById(int id)
        {
            return _context.TimeOffTypes.AsNoTracking().FirstOrDefault(s => s.TimeOffTypeId == id);
        }

        public bool IsUniqueTimeOffTypeName(TimeOffType timeOffType)
        {
            if (_context.TimeOffTypes.AsNoTracking().Count() == 0)
            {
                return true;
            }
            else if (timeOffType.TimeOffTypeId == 0)
            {
                return !_context.TimeOffTypes.AsNoTracking().Any(d => d.TimeOffTypeName.ToLower() == timeOffType.TimeOffTypeName.Trim().ToLower());
            }
            else
            {
                return !_context.TimeOffTypes.AsNoTracking().Any(d => d.TimeOffTypeName.ToLower() == timeOffType.TimeOffTypeName.Trim().ToLower() && d.TimeOffTypeId != timeOffType.TimeOffTypeId);
            }
        }

        public bool IsTimeOffTypeUsedInRequests(TimeOffType timeOffType)
        {
            return _context.TimeOffRequests.AsNoTracking().Any(d => d.TimeOffTypeId == timeOffType.TimeOffTypeId);
        }

        public List<TimeOffType> GetAllActiveTimeOffTypes()
        {
            return _context.TimeOffTypes.AsNoTracking().Where(t => t.IsActive == true).ToList();
        }

        // TimeOffRequests Read Operations
        public List<TimeOffRequest> GetAllTimeOffRequests()
        {
            return _context.TimeOffRequests.AsNoTracking().ToList();
        }

        public TimeOffRequest? GetTimeOffRequestById(int id)
        {
            return _context.TimeOffRequests.AsNoTracking().FirstOrDefault(s => s.TimeOffRequestId == id);
        }

        // TimeOffBalance Read Operations

        public List<TimeOffBalance> GetAllTimeOffBalances()
        {
            return _context.TimeOffBalances.AsNoTracking().ToList();
        }

        public TimeOffBalance? GetTimeOffBalance(int employeeId, int timeOffTypeId)
        {
            TimeOffBalance? timeOffBalance = _context.TimeOffBalances
                .AsNoTracking()
                .FirstOrDefault(t => t.EmployeeId == employeeId && t.TimeOffTypeId == timeOffTypeId);

            if(timeOffBalance == null)
            {
                timeOffBalance = new TimeOffBalance
                {
                    EmployeeId = employeeId,
                    TimeOffTypeId = timeOffTypeId,
                    TotalDays = 0,
                    UsedDays = 0,
                    RemainingDays = 0
                };
            }
            return timeOffBalance;
        }

        // Role Read Operations

        public List<Role> GetAllRolesByUserId(int userId)
        {
            return _context.Roles.AsNoTracking().Where(r => r.Users.Any(u => u.UserId == userId)).ToList();
        }

        public List<Role> GetAllRolesByEmployeeId(int employeeId)
        {
            return _context.Roles.AsNoTracking().Where(r => r.Users.Any(u => u.Employee != null && u.Employee.EmployeeId == employeeId)).ToList();
        }

        public List<Role> GetAllRoles()
        {
            return _context.Roles.AsNoTracking().ToList();
        }

        public bool IsUniqueRoleName(Role role)
        {
            if (_context.Roles.AsNoTracking().Count() == 0)
            {
                return true;
            }
            else if (role.RoleId == 0)
            {
                return !_context.Roles.AsNoTracking().Any(d => d.RoleName.ToLower() == role.RoleName.Trim().ToLower());
            }
            else
            {
                return !_context.Roles.AsNoTracking().Any(d => d.RoleName.ToLower() == role.RoleName.Trim().ToLower() && d.RoleId != role.RoleId);
            }
        }

        public bool IsRoleUsedInUsers(int roleId)
        {
            return _context.Users.AsNoTracking().Any(d => d.RoleId == roleId);
        }

        // Permission Read Operations
        public List<Permission> GetAllPermissions()
        {
            return _context.Permissions.AsNoTracking().ToList();
        }

        // PayPeriod Read Operations
        public List<PayPeriod> GetAllPayPeriods()
        {
            return _context.PayPeriods.AsNoTracking().ToList();
        }

        public PayPeriod? GetPayPeriodById(int id)
        {
            return _context.PayPeriods.AsNoTracking().FirstOrDefault(s => s.PayPeriodId == id);
        }

        public bool IsPayPeriodTimeValid(PayPeriod payPeriod)
        {
            // Check date logic
            if (payPeriod.StartDate > payPeriod.EndDate)
                return false;

            if (payPeriod.PaymentDate < payPeriod.EndDate)
                return false;

            // Check for overlapping PayPeriods (excluding itself if updating)
            var overlappingPeriods = _context.PayPeriods
                .AsNoTracking()
                .Where(p => p.PayPeriodId != payPeriod.PayPeriodId)
                .Where(p =>
                    (payPeriod.StartDate >= p.StartDate && payPeriod.StartDate <= p.EndDate) ||
                    (payPeriod.EndDate >= p.StartDate && payPeriod.EndDate <= p.EndDate) ||
                    (payPeriod.StartDate <= p.StartDate && payPeriod.EndDate >= p.EndDate))
                .Any();

            return !overlappingPeriods;
        }

        // SystemLog Read Operations
        public List<SystemLog> GetAllSystemLogs()
        {
            return _context.SystemLogs.AsNoTracking().ToList();
        }

        public bool CanDeletePayPeriod(PayPeriod payPeriod)
        {
            // Check if there are any EmployeePayrolls linked to this PayPeriod
            return (payPeriod.Status != "Completed") || (payPeriod.Status == "Completed" && !_context.EmployeePayrolls.AsNoTracking().Any(ep => ep.PayPeriodId == payPeriod.PayPeriodId));
        }

        // PerformanceCriteria Read Operations

        public List<PerformanceCriterion> GetAllPerformanceCriterias()
        {
            return _context.PerformanceCriteria.AsNoTracking().ToList();
        }

        public bool IsUniquePerformanceCriterionName(PerformanceCriterion performanceCriterion)
        {
            if (_context.PerformanceCriteria.AsNoTracking().Count() == 0)
            {
                return true;
            }
            else if (performanceCriterion.CriteriaId == 0)
            {
                return !_context.PerformanceCriteria.AsNoTracking().Any(d => d.CriteriaName.ToLower() == performanceCriterion.CriteriaName.Trim().ToLower());
            }
            else
            {
                return !_context.PerformanceCriteria.AsNoTracking().Any(d => d.CriteriaName.ToLower() == performanceCriterion.CriteriaName.Trim().ToLower() && d.CriteriaId != performanceCriterion.CriteriaId);
            }
        }

        public bool IsPerformanceCriterionUsedInScores(int performanceCriterionId)
        {
            return _context.PerformanceScores.AsNoTracking().Any(d => d.CriteriaId == performanceCriterionId);
        }

        public List<PerformanceCriterion> GetAllActiveCriteria()
        {
            return _context.PerformanceCriteria.AsNoTracking().Where(c => c.IsActive == true).ToList();
        }

        // PerformanceReview Read Operations

        public List<PerformanceReview> GetAllPerformanceReviews()
        {
            return _context.PerformanceReviews.AsNoTracking().ToList();
        }

        public PerformanceReview? GetPerformanceReviewById(int id)
        {
            return _context.PerformanceReviews.AsNoTracking().FirstOrDefault(s => s.ReviewId == id);
        }

        // PerformanceScore Read Operations

        public List<PerformanceScore> GetAllPerformanceScores()
        {
            return _context.PerformanceScores.AsNoTracking().ToList();
        }

        public PerformanceScore? GetPerformanceScoreById(int id)
        {
            return _context.PerformanceScores.AsNoTracking().FirstOrDefault(s => s.ScoreId == id);
        }

        // PayrollItems Read Operations

        public List<PayrollItem> GetAllPayrollItems()
        {
            return _context.PayrollItems.AsNoTracking().ToList();
        }

        public bool IsUniquePayrollItemName(PayrollItem payrollItem)
        {
            if (_context.PayrollItems.AsNoTracking().Count() == 0)
            {
                return true;
            }
            else if (payrollItem.PayrollItemId == 0)
            {
                return !_context.PayrollItems.AsNoTracking().Any(d => d.ItemName.ToLower() == payrollItem.ItemName.Trim().ToLower());
            }
            else
            {
                return !_context.PayrollItems.AsNoTracking().Any(d => d.ItemName.ToLower() == payrollItem.ItemName.Trim().ToLower() && d.PayrollItemId != payrollItem.PayrollItemId);
            }
        }

        public bool IsPayrollItemUsedInPayrollDetails(int payrollItemId)
        {
            return _context.PayrollDetails.AsNoTracking().Any(d => d.PayrollItemId == payrollItemId);
        }

        public PayrollItem? GetPayrollItemById(int id)
        {
            return _context.PayrollItems.AsNoTracking().FirstOrDefault(s => s.PayrollItemId == id);
        }

        // EmployeePayroll Read Operations

        public List<EmployeePayroll> GetAllEmployeePayrolls()
        {
            return _context.EmployeePayrolls.AsNoTracking().ToList();
        }

        public EmployeePayroll? GetEmployeePayrollById(int id)
        {
            return _context.EmployeePayrolls.AsNoTracking().FirstOrDefault(s => s.PayrollId == id);
        }

        // EmployeePayroll Read Operations by EmployeeId

        public List<EmployeePayroll> GetEmployeePayrollsByEmployeeId(int employeeId)
        {
            return _context.EmployeePayrolls.AsNoTracking().Where(s => s.EmployeeId == employeeId).ToList();
        }

        public EmployeePayroll? GetEmployeePayrollByEmployeeAndPayPeriodId(int payPeriodId, int employeeId)
        {
            return _context.EmployeePayrolls.AsNoTracking().FirstOrDefault(s => s.EmployeeId == employeeId && s.PayPeriodId == payPeriodId);
        }

        // PayrollDetail Read Operations

        public List<PayrollDetail> GetAllPayrollDetails()
        {
            return _context.PayrollDetails.AsNoTracking().ToList();
        }

        public PayrollDetail? GetPayrollDetailById(int id)
        {
            return _context.PayrollDetails.AsNoTracking().FirstOrDefault(s => s.PayrollDetailId == id);
        }

        public List<PayrollDetail> GetPayrollDetailsByPayrollId(int payrollId)
        {
            return _context.PayrollDetails.AsNoTracking().Where(s => s.PayrollId == payrollId).ToList();
        }


        // =====================================
        // CREATE OPERATIONS
        // =====================================

        // User Create Operations
        public void AddUser(User actingUser, User newUser)
        {
            try
            {
                if (!HasPermission(actingUser, "ManageUsers"))
                    throw new UnauthorizedAccessException("You do not have permission to create users.");

                if (newUser == null)
                    throw new ArgumentNullException(nameof(newUser), "New user cannot be null.");

                using var context = new HumanResourcesDbContext();
                using var transaction = context.Database.BeginTransaction();

                if (newUser.Employee != null)
                {
                    // Fetch and attach the existing employee to avoid tracking conflict
                    var employee = context.Employees
                        .FirstOrDefault(e => e.EmployeeId == newUser.Employee.EmployeeId && e.User == null);

                    if (employee != null)
                    {
                        employee.User = newUser;
                        newUser.Employee = employee;
                    }
                    else
                    {
                        newUser.Employee = null;
                    }
                }

                context.Users.Add(newUser);
                context.SaveChanges();
                transaction.Commit();

                LogOperation(actingUser, "Create", nameof(User), newUser.UserId, null, newUser);
            }
            catch (Exception ex)
            {
                LogError(actingUser, "AddUser", ex);
            }
        }

        // Department Create Operations
        public void AddDepartment(User user, Department department)
        {
            try
            {
                if (!HasPermission(user, "SystemSettings"))
                    throw new UnauthorizedAccessException("You do not have permission to create departments.");

                _context.Departments.Add(department);
                _context.SaveChanges();
                LogOperation(user, "Create", nameof(Department), department.DepartmentId, null, department);
            }
            catch (Exception ex)
            {
                LogError(user, "AddDepartment", ex);
            }
        }
        // Employee Create Operations
        public void AddEmployee(User user, Employee employee)
        {
            try
            {
                if (!HasPermission(user, "CreateEmployees"))
                    throw new UnauthorizedAccessException("You do not have permission to create employees.");

                using var context = new HumanResourcesDbContext();
                using var transaction = context.Database.BeginTransaction();

                // Add the new employee
                context.Employees.Add(employee);
                context.SaveChanges(); // Required to get the generated EmployeeId

                // Fetch active time off types
                var allTypes = GetAllTimeOffTypes();

                var now = DateTime.Now;

                // Create balance entries for each time off type
                foreach (var type in allTypes)
                {
                    // Updated line to handle the null reference issue
                    var period = context.TimeOffBalances.FirstOrDefault(s => s.TimeOffTypeId == type.TimeOffTypeId)?.Period ?? "Yearly";

                    context.TimeOffBalances.Add(new TimeOffBalance
                    {
                        EmployeeId = employee.EmployeeId,
                        TimeOffTypeId = type.TimeOffTypeId,
                        Period = period,
                        TotalDays = type.DefaultDays ?? 0,
                        UsedDays = 0,
                        RemainingDays = type.DefaultDays ?? 0,
                        CreatedAt = now
                    });
                }

                context.SaveChanges();
                transaction.Commit();
                LogOperation(user, "Create", nameof(Employee), employee.EmployeeId, null, employee);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding employee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // Position Create Operations
        public void AddPosition(User user, Position position)
        {
            try
            {
                if (!HasPermission(user, "SystemSettings"))
                    throw new UnauthorizedAccessException("You do not have permission to create positions.");

                _context.Positions.Add(position);
                _context.SaveChanges();
                LogOperation(user, "Create", nameof(Position), position.PositionId, null, position);
            }
            catch (Exception ex)
            {
                LogError(user, "AddPosition", ex);
            }
        }
        // Attendance Create Operations
        public void AddAttendance(User user, Attendance attendance)
        {
            try
            {
                if (!HasPermission(user, "ManageAttendance"))
                    throw new UnauthorizedAccessException("You do not have permission to manage attendance.");

                _context.Attendances.Add(attendance);
                _context.SaveChanges();
                LogOperation(user, "Create", nameof(Attendance), attendance.AttendanceId, null, attendance);
            }
            catch (Exception ex)
            {
                LogError(user, "AddAttendance", ex);
            }
        }

        public void CheckIn(User user, Attendance attendance)
        {
            try
            {
                _context.Attendances.Add(attendance);
                _context.SaveChanges();
                LogOperation(user, "Create", nameof(Attendance), attendance.AttendanceId, null, attendance);
            }
            catch (Exception ex)
            {
                LogError(user, "AddAttendance", ex);
            }
        }

        // TimeOffTypes Create Operations
        public void AddTimeOffType(User user, TimeOffType timeOffType)
        {
            try
            {
                if (!HasPermission(user, "ManageLeaves"))
                    throw new UnauthorizedAccessException("You do not have permission to manage time off types.");

                _context.TimeOffTypes.Add(timeOffType);
                _context.SaveChanges();
                LogOperation(user, "Create", nameof(TimeOffType), timeOffType.TimeOffTypeId, null, timeOffType);
            }
            catch (Exception ex)
            {
                LogError(user, "AddTimeOffType", ex);
            }
        }
        // TimeOffRequests Create Operations
        public void AddTimeOffRequest(User user, TimeOffRequest request)
        {
            try
            {
                if (!HasPermission(user, "ManageLeaves"))
                    throw new UnauthorizedAccessException("You do not have permission to manage time off requests.");

                _context.TimeOffRequests.Add(request);
                _context.SaveChanges();
                LogOperation(user, "Create", nameof(TimeOffRequest), request.TimeOffRequestId, null, request);
            }
            catch (Exception ex)
            {
                LogError(user, "AddTimeOffRequest", ex);
            }
        }
        // TimeOffBalance Create Operations
        public void CreateTimeOffBalanceByTimeOffType(User user, TimeOffType timeOffType, string period)
        {
            try
            {
                if (!HasPermission(user, "ManageLeaves"))
                    throw new UnauthorizedAccessException("You do not have permission to manage time off balances.");

                var employees = _context.Employees.AsNoTracking().ToList();
                foreach (var employee in employees)
                {
                    var balance = new TimeOffBalance
                    {
                        EmployeeId = employee.EmployeeId,
                        TimeOffTypeId = timeOffType.TimeOffTypeId,
                        Period = period,
                        TotalDays = timeOffType.DefaultDays ?? 0,
                        UsedDays = 0,
                        RemainingDays = timeOffType.DefaultDays ?? 0,
                        CreatedAt = DateTime.Now
                    };
                    _context.TimeOffBalances.Add(balance);
                }
                _context.SaveChanges();
                LogOperation(user, "Create", nameof(TimeOffBalance), null, null, timeOffType);
            }
            catch (Exception ex)
            {
                LogError(user, "CreateTimeOffBalanceByTimeOffType", ex);
            }
        }
        //Role Create Operations
        public void CreateRole(User user, Role role)
        {
            try
            {
                if (!HasPermission(user, "ManageRoles"))
                    throw new UnauthorizedAccessException("You do not have permission to create roles.");

                _context.Roles.Add(role);
                _context.SaveChanges();
                LogOperation(user, "Create", nameof(Role), role.RoleId, null, role);
            }
            catch (Exception ex)
            {
                LogError(user, "CreateRole", ex);
            }
        }
        // PayPeriod Create Operations
        public void CreatePayPeriod(User user, PayPeriod payPeriod)
        {
            try
            {
                if (!HasPermission(user, "ProcessPayroll"))
                    throw new UnauthorizedAccessException("You do not have permission to create pay periods.");

                _context.PayPeriods.Add(payPeriod);
                _context.SaveChanges();
                LogOperation(user, "Create", nameof(PayPeriod), payPeriod.PayPeriodId, null, payPeriod);
            }
            catch (Exception ex)
            {
                LogError(user, "CreatePayPeriod", ex);
            }
        }
        // PerformanceCriteria Create Operations
        public void CreatePerformanceCriterion(User user, PerformanceCriterion criterion)
        {
            try
            {
                if (!HasPermission(user, "ManagePerformance"))
                    throw new UnauthorizedAccessException("You do not have permission to create performance criteria.");

                _context.PerformanceCriteria.Add(criterion);
                _context.SaveChanges();
                LogOperation(user, "Create", nameof(PerformanceCriterion), criterion.CriteriaId, null, criterion);
            }
            catch (Exception ex)
            {
                LogError(user, "CreatePerformanceCriterion", ex);
            }
        }
        // PerformanceReview Create Operations
        public void CreatePerformanceReview(User user, PerformanceReview review)
        {
            try
            {
                if (!HasPermission(user, "ManagePerformance"))
                    throw new UnauthorizedAccessException("You do not have permission to create performance reviews.");

                _context.PerformanceReviews.Add(review);
                _context.SaveChanges();
                LogOperation(user, "Create", nameof(PerformanceReview), review.ReviewId, null, review);
            }
            catch (Exception ex)
            {
                LogError(user, "CreatePerformanceReview", ex);
            }
        }
        // PayrollItems Create Operations
        public void CreatePayrollItem(User user, PayrollItem item)
        {
            try
            {
                if (!HasPermission(user, "ProcessPayroll"))
                    throw new UnauthorizedAccessException("You do not have permission to create payroll items.");

                _context.PayrollItems.Add(item);
                _context.SaveChanges();
                LogOperation(user, "Create", nameof(PayrollItem), item.PayrollItemId, null, item);
            }
            catch (Exception ex)
            {
                LogError(user, "CreatePayrollItem", ex);
            }
        }
        // EmployeePayroll Create Operations
        public void CreateEmployeePayroll(User user, EmployeePayroll payroll)
        {
            try
            {
                if (!HasPermission(user, "ProcessPayroll"))
                    throw new UnauthorizedAccessException("You do not have permission to create employee payroll records.");

                _context.EmployeePayrolls.Add(payroll);
                _context.SaveChanges();
                LogOperation(user, "Create", nameof(EmployeePayroll), payroll.PayrollId, null, payroll);
            }
            catch (Exception ex)
            {
                LogError(user, "CreateEmployeePayroll", ex);
            }
        }

        public int CreateEmployeePayrollReturnId(User user, EmployeePayroll payroll)
        {
            try
            {
                if (!HasPermission(user, "ProcessPayroll"))
                    throw new UnauthorizedAccessException("You do not have permission to create employee payroll records.");

                _context.EmployeePayrolls.Add(payroll);
                _context.SaveChanges();
                LogOperation(user, "Create", nameof(EmployeePayroll), payroll.PayrollId, null, payroll);
                return payroll.PayrollId;
            }
            catch (Exception ex)
            {
                LogError(user, "CreateEmployeePayrollReturnId", ex);
                return -1;
            }
        }
        // PayrollDetail Create Operations
        public void CreatePayrollDetail(User user, PayrollDetail detail)
        {
            try
            {
                if (!HasPermission(user, "ProcessPayroll"))
                    throw new UnauthorizedAccessException("You do not have permission to create payroll details.");

                _context.PayrollDetails.Add(detail);
                _context.SaveChanges();
                LogOperation(user, "Create", nameof(PayrollDetail), detail.PayrollDetailId, null, detail);
            }
            catch (Exception ex)
            {
                LogError(user, "CreatePayrollDetail", ex);
            }
        }
        // =====================================
        // UPDATE OPERATIONS
        // =====================================

        // User Update Operations
        public void UpdateLastLogin(int userId)
        {
            using (var context = new HumanResourcesDbContext())
            {
                var user = context.Users.Find(userId);
                if (user != null)
                {
                    user.LastLogin = DateTime.Now;
                    context.SaveChanges();
                }
            }
        }

        public void UpdateRole(User user, Role role)
        {
            using (var context = new HumanResourcesDbContext())
            {
                var local = context.Users.Local.FirstOrDefault(e => e.UserId == user.UserId);
                if (local != null)
                {
                    context.Entry(local).State = EntityState.Detached;
                }

                user.Role = role;
                context.Entry(user).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        // Department Update Operations
        public void UpdateDepartment(Department department)
        {
            using (var context = new HumanResourcesDbContext())
            {
                var local = context.Departments.Local
                    .FirstOrDefault(e => e.DepartmentId == department.DepartmentId);

                if (local != null)
                {
                    context.Entry(local).State = EntityState.Detached;
                }

                context.Entry(department).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        // Employee Update Operations
        public void UpdateEmployee(Employee employee)
        {
            using (var context = new HumanResourcesDbContext())
            {
                var local = context.Employees.Local
                    .FirstOrDefault(e => e.EmployeeId == employee.EmployeeId);

                if (local != null)
                {
                    context.Entry(local).State = EntityState.Detached;
                }

                context.Entry(employee).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        // Position Update Operations
        public void UpdatePosition(Position position)
        {
            using (var context = new HumanResourcesDbContext())
            {
                var local = context.Positions.Local
                    .FirstOrDefault(e => e.PositionId == position.PositionId);

                if (local != null)
                {
                    context.Entry(local).State = EntityState.Detached;
                }

                context.Entry(position).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        // Attendance Update Operations

        // TimeOffTypes Update Operations

        public void UpdateTimeOffType(TimeOffType timeOffType)
        {
            using (var context = new HumanResourcesDbContext())
            {
                var local = context.Set<TimeOffType>()
                    .Local
                    .FirstOrDefault(e => e.TimeOffTypeId == timeOffType.TimeOffTypeId);

                // If entity is tracked, detach it
                if (local != null)
                {
                    context.Entry(local).State = EntityState.Detached;
                }

                // Attach and mark as modified
                context.Entry(timeOffType).State = EntityState.Modified;

                // Save changes
                context.SaveChanges();
            }
        }

        // TimeOffRequests Update Operations

        public void UpdateTimeOffRequest(TimeOffRequest timeOffRequest)
        {
            try
            {
                using (var context = new HumanResourcesDbContext())
                {
                    var local = context.Set<TimeOffRequest>()
                        .Local
                        .FirstOrDefault(e => e.TimeOffRequestId == timeOffRequest.TimeOffRequestId);
                    // If entity is tracked, detach it
                    if (local != null)
                    {
                        context.Entry(local).State = EntityState.Detached;
                    }
                    // Attach and mark as modified
                    context.Entry(timeOffRequest).State = EntityState.Modified;
                    // Save changes
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating TimeOffRequest: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // TimeOffBalance Update Operations

        public void UpdateTimeOffBalance(TimeOffBalance timeOffBalance)
        {
            try
            {
                using (var context = new HumanResourcesDbContext())
                {
                    var local = context.Set<TimeOffBalance>()
                        .Local
                        .FirstOrDefault(e => e.TimeOffBalanceId == timeOffBalance.TimeOffBalanceId);
                    // If entity is tracked, detach it
                    if (local != null)
                    {
                        context.Entry(local).State = EntityState.Detached;
                    }
                    // Attach and mark as modified
                    context.Entry(timeOffBalance).State = EntityState.Modified;
                    // Save changes
                    context.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error updating TimeOffBalance: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        public void UpdateTimeOffBalancePeriod(TimeOffType type, string period)
        {
            try
            {
                using (var context = new HumanResourcesDbContext())
                {
                    using var transaction = context.Database.BeginTransaction();

                    var balances = context.TimeOffBalances
                        .Where(b => b.TimeOffTypeId == type.TimeOffTypeId)
                        .ToList();

                    foreach (var balance in balances)
                    {
                        balance.Period = period;
                        context.TimeOffBalances.Update(balance);
                    }

                    context.SaveChanges();
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating TimeOffBalance period: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Role Update Operations
        public void UpdateRole(Role role)
        {
            try
            {
                using (var context = new HumanResourcesDbContext())
                {
                    using var transaction = context.Database.BeginTransaction();

                    // Find the existing role with its permissions
                    var existingRole = context.Roles
                        .Include(r => r.RolePermissions)
                        .FirstOrDefault(r => r.RoleId == role.RoleId);

                    if (existingRole != null)
                    {
                        // Update basic role properties
                        existingRole.RoleName = role.RoleName;
                        existingRole.Description = role.Description;

                        // Remove all existing role permissions
                        context.RolePermissions.RemoveRange(existingRole.RolePermissions);

                        // Add the new role permissions
                        foreach (var permission in role.RolePermissions)
                        {
                            context.RolePermissions.Add(new RolePermission
                            {
                                RoleId = existingRole.RoleId,
                                PermissionId = permission.PermissionId,
                                CreatedAt = DateTime.Now
                            });
                        }

                        context.SaveChanges();
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating role: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // User Update Operations
        public void UpdateUser(User user)
        {
            try
            {
                using (var context = new HumanResourcesDbContext())
                {
                    using var transaction = context.Database.BeginTransaction();

                    // Detach local User
                    var localUser = context.Users.Local
                        .FirstOrDefault(e => e.UserId == user.UserId);
                    if (localUser != null)
                    {
                        context.Entry(localUser).State = EntityState.Detached;
                    }

                    // Remove the UserId from any other Employee that might already be linked to this UserId
                    int? currentEmployeeId = user.Employee?.EmployeeId;

                    var existingEmployeeWithUser = context.Employees
                        .FirstOrDefault(e => e.UserId == user.UserId && e.EmployeeId != currentEmployeeId);


                    if (existingEmployeeWithUser != null)
                    {
                        existingEmployeeWithUser.UserId = null;
                        context.Entry(existingEmployeeWithUser).State = EntityState.Modified;
                    }

                    // Handle Employee
                    if (user.Employee != null)
                    {
                        var localEmployee = context.Employees.Local
                            .FirstOrDefault(e => e.EmployeeId == user.Employee.EmployeeId);
                        if (localEmployee != null)
                        {
                            context.Entry(localEmployee).State = EntityState.Detached;
                        }

                        user.Employee.UserId = user.UserId;

                        context.Attach(user.Employee);
                        context.Entry(user.Employee).State = EntityState.Modified;
                    }

                    // Update User
                    context.Attach(user);
                    context.Entry(user).State = EntityState.Modified;

                    context.SaveChanges();
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // PayPeriod Update Operations

        public void UpdatePayPeriod(PayPeriod payPeriod)
        {
            try
            {
                using (var context = new HumanResourcesDbContext())
                {
                    var local = context.Set<PayPeriod>()
                        .Local
                        .FirstOrDefault(e => e.PayPeriodId == payPeriod.PayPeriodId);
                    // If entity is tracked, detach it
                    if (local != null)
                    {
                        context.Entry(local).State = EntityState.Detached;
                    }
                    // Attach and mark as modified
                    context.Entry(payPeriod).State = EntityState.Modified;
                    // Save changes
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating PayPeriod: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // PerformanceCriteria Update Operations

        public void UpdatePerformanceCriterion(PerformanceCriterion performanceCriterion)
        {
            try
            {
                using (var context = new HumanResourcesDbContext())
                {
                    var local = context.Set<PerformanceCriterion>()
                        .Local
                        .FirstOrDefault(e => e.CriteriaId == performanceCriterion.CriteriaId);
                    // If entity is tracked, detach it
                    if (local != null)
                    {
                        context.Entry(local).State = EntityState.Detached;
                    }
                    // Attach and mark as modified
                    context.Entry(performanceCriterion).State = EntityState.Modified;
                    // Save changes
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating PerformanceCriterion: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // PerformanceReview Update Operations
        public void UpdatePerformanceReview(PerformanceReview performanceReview)
        {
            try
            {
                using (var context = new HumanResourcesDbContext())
                {
                    var local = context.Set<PerformanceReview>()
                        .Local
                        .FirstOrDefault(e => e.ReviewId == performanceReview.ReviewId);
                    // If entity is tracked, detach it
                    if (local != null)
                    {
                        context.Entry(local).State = EntityState.Detached;
                    }
                    // Attach and mark as modified
                    context.Entry(performanceReview).State = EntityState.Modified;
                    // Save changes
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating PerformanceReview: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // PerformanceScore Update Operations
        public void UpdatePerformanceScore(PerformanceScore performanceScore)
        {
            try
            {
                using (var context = new HumanResourcesDbContext())
                {
                    var local = context.Set<PerformanceScore>()
                        .Local
                        .FirstOrDefault(e => e.ScoreId == performanceScore.ScoreId);
                    // If entity is tracked, detach it
                    if (local != null)
                    {
                        context.Entry(local).State = EntityState.Detached;
                    }
                    // Attach and mark as modified
                    context.Entry(performanceScore).State = EntityState.Modified;
                    // Save changes
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating PerformanceScore: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // PayrollItems Update Operations
        public void UpdatePayrollItem(PayrollItem payrollItem)
        {
            try
            {
                using (var context = new HumanResourcesDbContext())
                {
                    var local = context.Set<PayrollItem>()
                        .Local
                        .FirstOrDefault(e => e.PayrollItemId == payrollItem.PayrollItemId);
                    // If entity is tracked, detach it
                    if (local != null)
                    {
                        context.Entry(local).State = EntityState.Detached;
                    }
                    // Attach and mark as modified
                    context.Entry(payrollItem).State = EntityState.Modified;
                    // Save changes
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating PayrollItem: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // EmployeePayroll Update Operations

        public void UpdateEmployeePayroll(EmployeePayroll employeePayroll)
        {
            try
            {
                using (var context = new HumanResourcesDbContext())
                {
                    var local = context.Set<EmployeePayroll>()
                        .Local
                        .FirstOrDefault(e => e.PayrollId == employeePayroll.PayrollId);
                    // If entity is tracked, detach it
                    if (local != null)
                    {
                        context.Entry(local).State = EntityState.Detached;
                    }
                    // Attach and mark as modified
                    context.Entry(employeePayroll).State = EntityState.Modified;
                    // Save changes
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating EmployeePayroll: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdatePayrollDetail(PayrollDetail payrollDetail)
        {
            try
            {
                using (var context = new HumanResourcesDbContext())
                {
                    var local = context.Set<PayrollDetail>()
                        .Local
                        .FirstOrDefault(e => e.PayrollDetailId == payrollDetail.PayrollDetailId);
                    // If entity is tracked, detach it
                    if (local != null)
                    {
                        context.Entry(local).State = EntityState.Detached;
                    }
                    // Attach and mark as modified
                    context.Entry(payrollDetail).State = EntityState.Modified;
                    // Save changes
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating PayrollDetail: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Attendance Update Operations

        public void UpdateAttendance(Attendance attendance)
        {
            try
            {
                using (var context = new HumanResourcesDbContext())
                {
                    var local = context.Set<Attendance>()
                        .Local
                        .FirstOrDefault(e => e.AttendanceId == attendance.AttendanceId);
                    // If entity is tracked, detach it
                    if (local != null)
                    {
                        context.Entry(local).State = EntityState.Detached;
                    }
                    // Attach and mark as modified
                    context.Entry(attendance).State = EntityState.Modified;
                    // Save changes
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating Attendance: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void CheckOut(User user, Attendance attendance)
        {
            try
            {
                using (var context = new HumanResourcesDbContext())
                {
                    var local = context.Set<Attendance>()
                        .Local
                        .FirstOrDefault(e => e.AttendanceId == attendance.AttendanceId);
                    // If entity is tracked, detach it
                    if (local != null)
                    {
                        context.Entry(local).State = EntityState.Detached;
                    }
                    // Attach and mark as modified
                    context.Entry(attendance).State = EntityState.Modified;
                    // Save changes
                    context.SaveChanges();
                    LogOperation(user, "Update", nameof(Attendance), attendance.AttendanceId, null, attendance);
                }
            }
            catch (Exception ex)
            {
                LogError(user, "AddAttendance", ex);
            }
        }

        // =====================================
        // DELETE OPERATIONS
        // =====================================

        // Department Delete Operations
        public void DeleteDepartment(Department department)
        {
            var existingDepartment = _context.Departments.Find(department.DepartmentId);
            if (existingDepartment != null)
            {
                _context.Departments.Remove(existingDepartment);
                _context.SaveChanges();
            }
        }

        // Employee Delete Operations
        public void DeleteEmployee(Employee employee)
        {
            try
            {
                using (var context = new HumanResourcesDbContext())
                {
                    using var transaction = context.Database.BeginTransaction();

                    if (!CanDeleteEmployee(employee.EmployeeId))
                        throw new InvalidOperationException("This employee cannot be deleted due to dependencies.");

                    var existingEmployee = context.Employees.Find(employee.EmployeeId);
                    if (existingEmployee != null)
                    {

                        foreach (var timeOffBalance in existingEmployee.TimeOffBalances)
                        {
                            context.TimeOffBalances.Remove(timeOffBalance);
                        }

                        existingEmployee.TimeOffBalances.Clear();

                        foreach (var request in existingEmployee.TimeOffRequestEmployees)
                        {
                            context.TimeOffRequests.Remove(request);
                        }

                        existingEmployee.TimeOffRequestEmployees.Clear();

                        foreach (var attendance in existingEmployee.Attendances)
                        {
                            context.Attendances.Remove(attendance);
                        }

                        existingEmployee.Attendances.Clear();

                        foreach (var payroll in existingEmployee.EmployeePayrolls)
                        {
                            context.EmployeePayrolls.Remove(payroll);
                        }

                        existingEmployee.EmployeePayrolls.Clear();

                        foreach (var performanceReview in existingEmployee.PerformanceReviewEmployees)
                        {
                            context.PerformanceReviews.Remove(performanceReview);
                        }

                        existingEmployee.PerformanceReviewEmployees.Clear();

                        // Finally delete the employee
                        context.Employees.Remove(existingEmployee);
                        context.SaveChanges();
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting employee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Position Delete Operations
        public void DeletePosition(Position position)
        {
            var existingPosition = _context.Positions.Find(position.PositionId);
            if (existingPosition != null)
            {
                _context.Positions.Remove(existingPosition);
                _context.SaveChanges();
            }
        }

        // Attendance Delete Operations
        public void DeleteAttendance(Attendance attendance)
        {
            var existingAttendance = _context.Attendances.Find(attendance.AttendanceId);
            if (existingAttendance != null)
            {
                _context.Attendances.Remove(existingAttendance);
                _context.SaveChanges();
            }
        }

        // TimeOffTypes Delete Operations
        public void DeleteTimeOffType(TimeOffType timeOffType)
        {
            var existingTimeOffType = _context.TimeOffTypes.Find(timeOffType.TimeOffTypeId);
            if (existingTimeOffType != null)
            {
                _context.TimeOffTypes.Remove(existingTimeOffType);
                _context.SaveChanges();
            }
        }

        // TimeOffRequests Delete Operations
        public void DeleteTimeOffRequest(TimeOffRequest timeOffRequest)
        {
            var existingTimeOffRequest = _context.TimeOffRequests.Find(timeOffRequest.TimeOffRequestId);
            if (existingTimeOffRequest != null)
            {
                _context.TimeOffRequests.Remove(existingTimeOffRequest);
                _context.SaveChanges();
            }
        }

        // Roles Delete Operations
        public void DeleteRole(Role role)
        {
            try
            {
                using (var context = new HumanResourcesDbContext())
                {
                    using var transaction = context.Database.BeginTransaction();

                    // Find the role with its related permissions
                    var existingRole = context.Roles
                        .Include(r => r.RolePermissions)
                        .FirstOrDefault(r => r.RoleId == role.RoleId);

                    if (existingRole != null)
                    {
                        // First remove all role permissions
                        context.RolePermissions.RemoveRange(existingRole.RolePermissions);

                        // Then remove the role itself
                        context.Roles.Remove(existingRole);

                        context.SaveChanges();
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting role: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // User Delete Operations
        public void DeleteUser(User user)
        {
            try
            {
                using (var context = new HumanResourcesDbContext())
                {
                    using var transaction = context.Database.BeginTransaction();

                    var existingUser = context.Users.Find(user.UserId);
                    if (existingUser != null)
                    {
                        // Set UserId to null in related system logs
                        var relatedLogs = context.SystemLogs.Where(log => log.UserId == user.UserId).ToList();
                        foreach (var log in relatedLogs)
                        {
                            log.UserId = null;
                        }

                        // Set UserId to null in related payroll items
                        var relatedPayrolls = context.EmployeePayrolls.Where(p => p.ProcessedBy == user.UserId).ToList();
                        foreach (var payroll in relatedPayrolls)
                        {
                            payroll.ProcessedBy = null;

                        }

                        var relatedEmployees = context.Employees.Where(p => p.UserId == user.UserId).ToList();
                        foreach (var employee in relatedEmployees)
                        {
                            employee.UserId = null;

                        }

                        // Delete the user
                        context.Users.Remove(existingUser);
                        context.SaveChanges();
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // PayPeriod Delete Operations

        public void DeletePayPeriod(PayPeriod payPeriod)
        {
            try
            {
                using (var context = new HumanResourcesDbContext())
                {
                    using var transaction = context.Database.BeginTransaction();

                    var existingPayPeriod = context.PayPeriods.Find(payPeriod.PayPeriodId);
                    if (existingPayPeriod != null && CanDeletePayPeriod(existingPayPeriod))
                    {
                        var relatedEmployeePayrolls = context.EmployeePayrolls.Where(ep => ep.PayPeriodId == existingPayPeriod.PayPeriodId).ToList();
                        foreach (var payroll in relatedEmployeePayrolls)
                        {
                            context.RemoveRange(payroll.PayrollDetails);
                        }
                        context.RemoveRange(relatedEmployeePayrolls);

                        context.PayPeriods.Remove(existingPayPeriod);
                        context.SaveChanges();
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // PerformanceCriteria Delete Operations

        public void DeletePerformanceCriteria(PerformanceCriterion performanceCriterion)
        {
            var existingPerformanceCriterion = _context.PerformanceCriteria.Find(performanceCriterion.CriteriaId);
            if (existingPerformanceCriterion != null && !IsPerformanceCriterionUsedInScores(existingPerformanceCriterion.CriteriaId))
            {
                _context.PerformanceCriteria.Remove(existingPerformanceCriterion);
                _context.SaveChanges();
            }
        }

        // PerformanceReview Delete Operations

        public void DeletePerformanceReview(PerformanceReview performanceReview)
        {
            try
            {
                using (var context = new HumanResourcesDbContext())
                {
                    using var transaction = context.Database.BeginTransaction();

                    // Find the role with its related permissions
                    var existingPerformanceReview = context.PerformanceReviews.Find(performanceReview.ReviewId);

                    if (existingPerformanceReview != null)
                    {
                        context.PerformanceScores.RemoveRange(existingPerformanceReview.PerformanceScores);

                        context.PerformanceReviews.Remove(existingPerformanceReview);

                        context.SaveChanges();
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting performance review: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // PayrollItems Delete Operations
        public void DeletePayrollItem(PayrollItem payrollItem)
        {
            var existingPayrollItem = _context.PayrollItems.Find(payrollItem.PayrollItemId);
            if (existingPayrollItem != null)
            {
                _context.PayrollItems.Remove(existingPayrollItem);
                _context.SaveChanges();
            }
        }
    }
}