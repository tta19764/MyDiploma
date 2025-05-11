using Castle.DynamicProxy.Generators;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HumanResourcesApp.DBClasses
{
    internal class HumanResourcesDB
    {
        private HumanResourcesDbContext _context;
        public HumanResourcesDB()
        {
            _context = new HumanResourcesDbContext();
        }

        public bool HasPermission(User user, string permissionName)
        {
            return user.Role.RolePermissions.Any(rp => rp.Permission.PermissionName == permissionName);
        }

        

        public void LogOperation(User user, string action, string entityType, int? entityId, object? oldValues, object? newValues)
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
                    OldValues = oldValues != null ? JsonSerializer.Serialize(JSONLoggerHelper.TrimForLog(oldValues)) : null,
                    NewValues = newValues != null ? JsonSerializer.Serialize(JSONLoggerHelper.TrimForLog(newValues)) : null,
                    IpAddress = IpAddress.GetLocalIpAddress(),
                    UserAgent = Environment.MachineName
                };

                _context.SystemLogs.Add(log);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                TryFallbackLogging(user, action, entityType, entityId, ex);
            }
        }
        private void TryFallbackLogging(User user, string action, string entityType, int? entityId, Exception originalException)
        {
            try
            {
                // Create a simpler log entry without the problematic values
                var log = new SystemLog
                {
                    UserId = user.UserId,
                    LogDate = DateTime.Now,
                    LogLevel = "Warning",
                    LogSource = "HumanResourcesDB",
                    Action = action,
                    EntityType = entityType,
                    EntityId = entityId,
                    OldValues = "Serialization failed",
                    NewValues = $"Logging error: {originalException.Message}",
                    IpAddress = IpAddress.GetLocalIpAddress(),
                    UserAgent = Environment.MachineName
                };

                _context.SystemLogs.Add(log);
                _context.SaveChanges();
            }
            catch
            {
                // Could write to file or event log here
            }
        }

        public void LogError(User user, string sourceMethod, Exception ex)
        {
            try
            {
                // Create JSON serializer options
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                // Create a dictionary to store exception details
                var exceptionDetails = new Dictionary<string, string>
                {
                    ["ExceptionType"] = ex.GetType().FullName ?? "",
                    ["Message"] = ex.Message,
                    ["StackTrace"] = ex.StackTrace ?? "",
                    ["Source"] = ex.Source ?? "",
                };

                // Add inner exception details if present
                if (ex.InnerException != null)
                {
                    exceptionDetails["InnerExceptionType"] = ex.InnerException.GetType().FullName ?? "";
                    exceptionDetails["InnerMessage"] = ex.InnerException.Message;
                    exceptionDetails["InnerStackTrace"] = ex.InnerException.StackTrace ?? "";
                }

                // Convert to proper JSON string
                string jsonException = JsonSerializer.Serialize(exceptionDetails, options);

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
                    NewValues = jsonException, // Now properly formatted JSON
                    IpAddress = IpAddress.GetLocalIpAddress(),
                    UserAgent = Environment.MachineName
                };
                _context.SystemLogs.Add(log);
                _context.SaveChanges();
            }
            catch (Exception fallbackEx)
            {
                // Fallback if logging fails - write to console or file
                Console.WriteLine($"Failed to log error: {fallbackEx.Message}");

                try
                {
                    // Try a minimal log entry without the problematic JSON
                    var simpleLog = new SystemLog
                    {
                        UserId = user?.UserId ?? 0,
                        LogDate = DateTime.Now,
                        LogLevel = "Critical",
                        LogSource = $"HRMS::{sourceMethod}",
                        Action = "ExceptionLoggingFailed",
                        EntityType = null,
                        EntityId = null,
                        OldValues = null,
                        NewValues = JsonSerializer.Serialize(new { ErrorMessage = "Failed to log original error" }),
                        IpAddress = IpAddress.GetLocalIpAddress(),
                        UserAgent = Environment.MachineName
                    };
                    _context.SystemLogs.Add(simpleLog);
                    _context.SaveChanges();
                }
                catch
                {
                    // Could write to file or event log here
                }
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
                .Where(a => a.EmployeeId == attendance.EmployeeId && attendance.AttendanceId != a.AttendanceId)
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
                if (!HasPermission(user, "ManageLeaves") || user.Employee != null && user.Employee.EmployeeId == request.EmployeeId)
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

        // Department Update Operations
        public void UpdateDepartment(User user, Department updated)
        {
            try
            {
                if (!HasPermission(user, "SystemSettings"))
                    throw new UnauthorizedAccessException("You do not have permission to update departments.");

                var existing = _context.Departments.Find(updated.DepartmentId);
                if (existing != null)
                {
                    var oldValues = JSONLoggerHelper.TrimForLog(existing);

                    _context.Entry(existing).CurrentValues.SetValues(updated);
                    _context.SaveChanges();
                    LogOperation(user, "Update", nameof(Department), existing.DepartmentId, oldValues, updated);
                }
            }
            catch (Exception ex)
            {
                LogError(user, "UpdateDepartment", ex);
            }
        }

        // Employee Update Operations
        public void UpdateEmployee(User user, Employee updated)
        {
            try
            {
                if (!HasPermission(user, "EditEmployees"))
                    throw new UnauthorizedAccessException("You do not have permission to update positions.");

                var existing = _context.Employees.Find(updated.EmployeeId);
                if (existing != null)
                {
                    var oldValues = JSONLoggerHelper.TrimForLog(existing);

                    _context.Entry(existing).CurrentValues.SetValues(updated);
                    _context.SaveChanges();
                    LogOperation(user, "Update", nameof(Employee), existing.EmployeeId, oldValues, updated);
                }
            }
            catch (Exception ex)
            {
                LogError(user, "UpdateEmployee", ex);
            }
        }

        // Position Update Operations
        public void UpdatePosition(User user, Position updated)
        {
            try
            {
                if (!HasPermission(user, "SystemSettings"))
                    throw new UnauthorizedAccessException("You do not have permission to update positions.");

                var existing = _context.Positions.Find(updated.PositionId);
                if (existing != null)
                {
                    var oldValues = JSONLoggerHelper.TrimForLog(existing);

                    _context.Entry(existing).CurrentValues.SetValues(updated);
                    _context.SaveChanges();
                    LogOperation(user, "Update", nameof(Position), existing.PositionId, oldValues, updated);
                }
            }
            catch (Exception ex)
            {
                LogError(user, "UpdatePosition", ex);
            }
        }

        // Attendance Update Operations

        // TimeOffTypes Update Operations

        public void UpdateTimeOffType(User user, TimeOffType updated)
        {
            try
            {
                if (!HasPermission(user, "ManageLeaves"))
                    throw new UnauthorizedAccessException("You do not have permission to update time off types.");

                var existing = _context.TimeOffTypes.Find(updated.TimeOffTypeId);
                if (existing != null)
                {
                    var oldValues = JSONLoggerHelper.TrimForLog(existing);

                    _context.Entry(existing).CurrentValues.SetValues(updated);
                    _context.SaveChanges();
                    LogOperation(user, "Update", nameof(TimeOffType), existing.TimeOffTypeId, oldValues, updated);
                }
            }
            catch (Exception ex)
            {
                LogError(user, "UpdateTimeOffType", ex);
            }
        }

        // TimeOffRequests Update Operations
        public void UpdateTimeOffRequest(User user, TimeOffRequest timeOffRequest)
        {
            try
            {
                if (!HasPermission(user, "ManageLeaves"))
                    throw new UnauthorizedAccessException("You do not have permission to update time off requests.");

                // Fetch existing entity from DB and clone it for logging
                var existing = _context.TimeOffRequests
                    .AsNoTracking()
                    .FirstOrDefault(e => e.TimeOffRequestId == timeOffRequest.TimeOffRequestId);

                if (existing == null)
                    throw new InvalidOperationException("TimeOffRequest not found.");

                var oldValues = JSONLoggerHelper.TrimForLog(existing);

                // Detach local tracked entity if it exists
                var local = _context.Set<TimeOffRequest>()
                    .Local
                    .FirstOrDefault(e => e.TimeOffRequestId == timeOffRequest.TimeOffRequestId);

                if (local != null)
                {
                    _context.Entry(local).State = EntityState.Detached;
                }

                _context.Entry(timeOffRequest).State = EntityState.Modified;
                _context.SaveChanges();

                LogOperation(user, "Update", nameof(TimeOffRequest), timeOffRequest.TimeOffRequestId, oldValues, timeOffRequest);
            }
            catch (Exception ex)
            {
                LogError(user, "UpdateTimeOffRequest", ex);
            }
        }


        // TimeOffBalance Update Operations
        public void UpdateTimeOffBalance(User user, TimeOffBalance timeOffBalance)
        {
            try
            {
                if (!HasPermission(user, "ManageLeaves"))
                    throw new UnauthorizedAccessException("You do not have permission to update time off balances.");

                // Fetch existing entity from DB and clone it for logging
                var existing = _context.TimeOffBalances
                    .AsNoTracking()
                    .FirstOrDefault(e => e.TimeOffBalanceId == timeOffBalance.TimeOffBalanceId);

                if (existing == null)
                    throw new InvalidOperationException("TimeOffBalance not found.");

                var oldValues = JSONLoggerHelper.TrimForLog(existing);

                // Detach local tracked entity if it exists
                var local = _context.Set<TimeOffBalance>()
                    .Local
                    .FirstOrDefault(e => e.TimeOffBalanceId == timeOffBalance.TimeOffBalanceId);

                if (local != null)
                {
                    _context.Entry(local).State = EntityState.Detached;
                }

                _context.Entry(timeOffBalance).State = EntityState.Modified;
                _context.SaveChanges();

                LogOperation(user, "Update", nameof(TimeOffBalance), timeOffBalance.TimeOffBalanceId, oldValues, timeOffBalance);
            }
            catch (Exception ex)
            {
                LogError(user, "UpdateTimeOffBalance", ex);
            }
        }


        public void UpdateTimeOffBalancePeriod(User user, TimeOffType type, string period)
        {
            try
            {
                if (!HasPermission(user, "ManageLeaves"))
                    throw new UnauthorizedAccessException("You do not have permission to update time off balances.");

                using var transaction = _context.Database.BeginTransaction();

                var balances = _context.TimeOffBalances
                    .Where(b => b.TimeOffTypeId == type.TimeOffTypeId)
                    .ToList();

                foreach (var balance in balances)
                {
                    balance.Period = period;
                    _context.TimeOffBalances.Update(balance);
                }

                _context.SaveChanges();
                transaction.Commit();

                LogOperation(user, "Update", nameof(TimeOffBalance), null, null, $"Updated period to '{period}' for TimeOffTypeId {type.TimeOffTypeId}");
            }
            catch (Exception ex)
            {
                LogError(user, "UpdateTimeOffBalancePeriod", ex);
            }
        }

        // Role Update Operations
        public void UpdateRole(User user, Role updated)
        {
            try
            {
                if (!HasPermission(user, "ManageRoles"))
                    throw new UnauthorizedAccessException("You do not have permission to update roles.");

                var existing = _context.Roles.Find(updated.RoleId);
                if (existing != null)
                {
                    var oldValues = JSONLoggerHelper.TrimForLog(existing);

                    _context.Entry(existing).CurrentValues.SetValues(updated);
                    _context.SaveChanges();
                    LogOperation(user, "Update", nameof(Role), existing.RoleId, oldValues, updated);
                }
            }
            catch (Exception ex)
            {
                LogError(user, "UpdateRole", ex);
            }
        }

        // User Update Operations
        public void UpdateUser(User user, User updated)
        {
            try
            {
                if (!HasPermission(user, "ManageUsers"))
                    throw new UnauthorizedAccessException("You do not have permission to update users.");

                using var transaction = _context.Database.BeginTransaction();

                var localUser = _context.Users.Local
                    .FirstOrDefault(e => e.UserId == updated.UserId);
                if (localUser != null)
                {

                    _context.Entry(localUser).State = EntityState.Detached;
                }

                int? currentEmployeeId = updated.Employee?.EmployeeId;
                var existingEmployeeWithUser = _context.Employees
                    .FirstOrDefault(e => e.UserId == updated.UserId && e.EmployeeId != currentEmployeeId);

                if (existingEmployeeWithUser != null)
                {
                    existingEmployeeWithUser.UserId = null;
                    _context.Entry(existingEmployeeWithUser).State = EntityState.Modified;
                }

                if (updated.Employee != null)
                {
                    var localEmployee = _context.Employees.Local
                        .FirstOrDefault(e => e.EmployeeId == updated.Employee.EmployeeId);
                    if (localEmployee != null)
                    {
                        _context.Entry(localEmployee).State = EntityState.Detached;
                    }

                    updated.Employee.UserId = updated.UserId;
                    _context.Attach(updated.Employee);
                    _context.Entry(updated.Employee).State = EntityState.Modified;
                }

                _context.Attach(updated);
                _context.Entry(updated).State = EntityState.Modified;

                _context.SaveChanges();
                transaction.Commit();

                LogOperation(user, "Update", nameof(User), updated.UserId, null, updated);
            }
            catch (Exception ex)
            {
                LogError(user, "UpdateUser", ex);
            }
        }

        // PayPeriod Update Operations
        public void UpdatePayPeriod(User user, PayPeriod updated)
        {
            try
            {
                if (!HasPermission(user, "ProcessPayroll"))
                    throw new UnauthorizedAccessException("You do not have permission to update pay periods.");

                var existing = _context.PayPeriods.Find(updated.PayPeriodId);
                if (existing != null)
                {
                    var oldValues = JSONLoggerHelper.TrimForLog(existing);

                    _context.Entry(existing).CurrentValues.SetValues(updated);
                    _context.SaveChanges();
                    LogOperation(user, "Update", nameof(PayPeriod), existing.PayPeriodId, oldValues, updated);
                }
            }
            catch (Exception ex)
            {
                LogError(user, "UpdatePayPeriod", ex);
            }
        }

        // PerformanceCriteria Update Operations

        public void UpdatePerformanceCriterion(User user, PerformanceCriterion performanceCriterion)
        {
            try
            {
                if (!HasPermission(user, "ManagePerformance"))
                    throw new UnauthorizedAccessException("You do not have permission to update performance criteria.");

                // Retrieve the current values from the database (not tracked)
                var existing = _context.PerformanceCriteria
                    .AsNoTracking()
                    .FirstOrDefault(e => e.CriteriaId == performanceCriterion.CriteriaId);

                if (existing == null)
                    throw new InvalidOperationException("PerformanceCriterion not found.");

                var oldValues = JSONLoggerHelper.TrimForLog(existing);

                // Detach local tracked instance if it exists
                var local = _context.Set<PerformanceCriterion>()
                    .Local
                    .FirstOrDefault(e => e.CriteriaId == performanceCriterion.CriteriaId);

                if (local != null)
                {
                    _context.Entry(local).State = EntityState.Detached;
                }

                _context.Entry(performanceCriterion).State = EntityState.Modified;
                _context.SaveChanges();

                LogOperation(user, "Update", nameof(PerformanceCriterion), performanceCriterion.CriteriaId, oldValues, performanceCriterion);
            }
            catch (Exception ex)
            {
                LogError(user, "UpdatePerformanceCriterion", ex);
            }
        }


        // PerformanceReview Update Operations
        public void UpdatePerformanceReview(User user, PerformanceReview updated)
        {
            try
            {
                if (!HasPermission(user, "ManagePerformance"))
                    throw new UnauthorizedAccessException("You do not have permission to update performance reviews.");

                var existing = _context.PerformanceReviews.Find(updated.ReviewId);
                if (existing != null)
                {
                    var oldValues = JSONLoggerHelper.TrimForLog(existing);

                    _context.Entry(existing).CurrentValues.SetValues(updated);
                    _context.SaveChanges();
                    LogOperation(user, "Update", nameof(PerformanceReview), existing.ReviewId, oldValues, updated);
                }
            }
            catch (Exception ex)
            {
                LogError(user, "UpdatePerformanceReview", ex);
            }
        }

        // PerformanceScore Update Operations
        public void UpdatePerformanceScore(User user, PerformanceScore performanceScore)
        {
            try
            {
                if (!HasPermission(user, "ManagePerformance"))
                    throw new UnauthorizedAccessException("You do not have permission to update performance scores.");

                // Retrieve the current values from the database (not tracked)
                var existing = _context.PerformanceScores
                    .AsNoTracking()
                    .FirstOrDefault(e => e.ScoreId == performanceScore.ScoreId);

                if (existing == null)
                    throw new InvalidOperationException("PerformanceScore not found.");

                var oldValues = JSONLoggerHelper.TrimForLog(existing);

                // Detach local tracked instance if it exists
                var local = _context.Set<PerformanceScore>()
                    .Local
                    .FirstOrDefault(e => e.ScoreId == performanceScore.ScoreId);

                if (local != null)
                {
                    _context.Entry(local).State = EntityState.Detached;
                }

                _context.Entry(performanceScore).State = EntityState.Modified;
                _context.SaveChanges();

                LogOperation(user, "Update", nameof(PerformanceScore), performanceScore.ScoreId, oldValues, performanceScore);
            }
            catch (Exception ex)
            {
                LogError(user, "UpdatePerformanceScore", ex);
            }
        }


        // PayrollItems Update Operations
        public void UpdatePayrollItem(User user, PayrollItem updated)
        {
            try
            {
                if (!HasPermission(user, "ProcessPayroll"))
                    throw new UnauthorizedAccessException("You do not have permission to update payroll items.");

                var existing = _context.PayrollItems.Find(updated.PayrollItemId);
                if (existing != null)
                {
                    var oldValues = JSONLoggerHelper.TrimForLog(existing);

                    _context.Entry(existing).CurrentValues.SetValues(updated);
                    _context.SaveChanges();
                    LogOperation(user, "Update", nameof(PayrollItem), existing.PayrollItemId, oldValues, updated);
                }
            }
            catch (Exception ex)
            {
                LogError(user, "UpdatePayrollItem", ex);
            }
        }

        // EmployeePayroll Update Operations

        public void UpdateEmployeePayroll(User user, EmployeePayroll employeePayroll)
        {
            try
            {
                if (!HasPermission(user, "ProcessPayroll"))
                    throw new UnauthorizedAccessException("You do not have permission to update payrolls.");

                // Retrieve the current values from the database (not tracked)
                var existing = _context.EmployeePayrolls
                    .AsNoTracking()
                    .FirstOrDefault(e => e.PayrollId == employeePayroll.PayrollId);

                if (existing == null)
                    throw new InvalidOperationException("EmployeePayroll not found.");

                var oldValues = JSONLoggerHelper.TrimForLog(existing);

                var local = _context.Set<EmployeePayroll>()
                    .Local
                    .FirstOrDefault(e => e.PayrollId == employeePayroll.PayrollId);
                if (local != null)
                {
                    _context.Entry(local).State = EntityState.Detached;
                }

                _context.Entry(employeePayroll).State = EntityState.Modified;
                _context.SaveChanges();
                LogOperation(user, "Update", nameof(EmployeePayroll), employeePayroll.PayrollId, oldValues, employeePayroll);
            }
            catch (Exception ex)
            {
                LogError(user, "UpdateEmployeePayroll", ex);
            }
        }

        public void UpdatePayrollDetail(User user, PayrollDetail payrollDetail)
        {
            try
            {
                if (!HasPermission(user, "ProcessPayroll"))
                    throw new UnauthorizedAccessException("You do not have permission to update payroll details.");

                // Retrieve the current values from the database (not tracked)
                var existing = _context.PayrollDetails
                    .AsNoTracking()
                    .FirstOrDefault(e => e.PayrollDetailId == payrollDetail.PayrollDetailId);

                if (existing == null)
                    throw new InvalidOperationException("PerformanceScore not found.");

                var oldValues = JSONLoggerHelper.TrimForLog(existing);

                var local = _context.Set<PayrollDetail>()
                    .Local
                    .FirstOrDefault(e => e.PayrollDetailId == payrollDetail.PayrollDetailId);
                if (local != null)
                {
                    _context.Entry(local).State = EntityState.Detached;
                }

                _context.Entry(payrollDetail).State = EntityState.Modified;
                _context.SaveChanges();
                LogOperation(user, "Update", nameof(PayrollDetail), payrollDetail.PayrollDetailId, oldValues, payrollDetail);
            }
            catch (Exception ex)
            {
                LogError(user, "UpdatePayrollDetail", ex);
            }
        }


        // Attendance Update Operations
        public void UpdateAttendance(User user, Attendance updated)
        {
            try
            {
                if (!HasPermission(user, "ManageAttendance"))
                    throw new UnauthorizedAccessException("You do not have permission to update attendance records.");

                var existing = _context.Attendances.Find(updated.AttendanceId);
                if (existing != null)
                {
                    var oldValues = JSONLoggerHelper.TrimForLog(existing);

                    _context.Entry(existing).CurrentValues.SetValues(updated);
                    _context.SaveChanges();
                    LogOperation(user, "Update", nameof(Attendance), existing.AttendanceId, oldValues, updated);
                }
            }
            catch (Exception ex)
            {
                LogError(user, "UpdateAttendance", ex);
            }
        }

        public void CheckOut(User user, Attendance updated)
        {
            try
            {
                var existing = _context.Attendances.Find(updated.AttendanceId);
                if (existing != null)
                {
                    var oldValues = JSONLoggerHelper.TrimForLog(existing);
                    
                    _context.Entry(existing).CurrentValues.SetValues(updated);
                    _context.SaveChanges();
                    LogOperation(user, "Update", nameof(Attendance), existing.AttendanceId, oldValues, updated);
                }
            }
            catch (Exception ex)
            {
                LogError(user, "UpdateAttendance", ex);
            }
        }

        // =====================================
        // DELETE OPERATIONS
        // =====================================

        // Department Delete Operations
        public void DeleteDepartment(User user, Department department)
        {
            try
            {
                if (!HasPermission(user, "SystemSettings"))
                    throw new UnauthorizedAccessException("You do not have permission to delete departments.");

                var existing = _context.Departments.Find(department.DepartmentId);
                if (existing != null)
                {
                    _context.Departments.Remove(existing);
                    _context.SaveChanges();
                    LogOperation(user, "Delete", nameof(Department), existing.DepartmentId, existing, null);
                }
            }
            catch (Exception ex)
            {
                LogError(user, "DeleteDepartment", ex);
            }
        }

        // Employee Delete Operations
        public void DeleteEmployee(User user, Employee employee)
        {
            try
            {
                if (!HasPermission(user, "DeleteEmployees"))
                    throw new UnauthorizedAccessException("You do not have permission to delete employees.");

                using (var context = new HumanResourcesDbContext())
                {
                    using var transaction = context.Database.BeginTransaction();

                    if (!CanDeleteEmployee(employee.EmployeeId))
                        throw new InvalidOperationException("This employee cannot be deleted due to dependencies.");

                    var existing = context.Employees
                        .Include(e => e.TimeOffBalances)
                        .Include(e => e.TimeOffRequestEmployees)
                        .Include(e => e.Attendances)
                        .Include(e => e.EmployeePayrolls)
                        .Include(e => e.PerformanceReviewEmployees)
                        .FirstOrDefault(e => e.EmployeeId == employee.EmployeeId);

                    if (existing != null)
                    {
                        context.TimeOffBalances.RemoveRange(existing.TimeOffBalances);
                        context.TimeOffRequests.RemoveRange(existing.TimeOffRequestEmployees);
                        context.Attendances.RemoveRange(existing.Attendances);
                        context.EmployeePayrolls.RemoveRange(existing.EmployeePayrolls);
                        context.PerformanceReviews.RemoveRange(existing.PerformanceReviewEmployees);

                        context.Employees.Remove(existing);
                        context.SaveChanges();
                        transaction.Commit();

                        LogOperation(user, "Delete", nameof(Employee), existing.EmployeeId, existing, null);
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(user, "DeleteEmployee", ex);
            }
        }


        // Position Delete Operations
        public void DeletePosition(User user, Position position)
        {
            try
            {
                if (!HasPermission(user, "SystemSettings"))
                    throw new UnauthorizedAccessException("You do not have permission to delete positions.");

                var existing = _context.Positions.Find(position.PositionId);
                if (existing != null)
                {
                    _context.Positions.Remove(existing);
                    _context.SaveChanges();
                    LogOperation(user, "Delete", nameof(Position), existing.PositionId, existing, null);
                }
            }
            catch (Exception ex)
            {
                LogError(user, "DeletePosition", ex);
            }
        }

        // Attendance Delete Operations
        public void DeleteAttendance(User user, Attendance attendance)
        {
            try
            {
                if (!HasPermission(user, "ManageAttendance"))
                    throw new UnauthorizedAccessException("You do not have permission to delete attendance records.");

                var existing = _context.Attendances.Find(attendance.AttendanceId);
                if (existing != null)
                {
                    _context.Attendances.Remove(existing);
                    _context.SaveChanges();
                    LogOperation(user, "Delete", nameof(Attendance), existing.AttendanceId, existing, null);
                }
            }
            catch (Exception ex)
            {
                LogError(user, "DeleteAttendance", ex);
            }
        }

        // TimeOffTypes Delete Operations
        public void DeleteTimeOffType(User user, TimeOffType type)
        {
            try
            {
                if (!HasPermission(user, "ManageLeaves"))
                    throw new UnauthorizedAccessException("You do not have permission to delete time off types.");

                var existing = _context.TimeOffTypes.Find(type.TimeOffTypeId);
                if (existing != null)
                {
                    _context.TimeOffTypes.Remove(existing);
                    _context.SaveChanges();
                    LogOperation(user, "Delete", nameof(TimeOffType), existing.TimeOffTypeId, existing, null);
                }
            }
            catch (Exception ex)
            {
                LogError(user, "DeleteTimeOffType", ex);
            }
        }

        // TimeOffRequests Delete Operations
        public void DeleteTimeOffRequest(User user, TimeOffRequest request)
        {
            try
            {
                if (!HasPermission(user, "ManageLeaves") || !(user.Employee != null && user.Employee.EmployeeId == request.EmployeeId))
                    throw new UnauthorizedAccessException("You do not have permission to delete time off requests.");

                var existing = _context.TimeOffRequests.Find(request.TimeOffRequestId);
                if (existing != null)
                {
                    _context.TimeOffRequests.Remove(existing);
                    _context.SaveChanges();
                    LogOperation(user, "Delete", nameof(TimeOffRequest), existing.TimeOffRequestId, existing, null);
                }
            }
            catch (Exception ex)
            {
                LogError(user, "DeleteTimeOffRequest", ex);
            }
        }

        // Roles Delete Operations
        public void DeleteRole(User user, Role role)
        {
            try
            {
                if (!HasPermission(user, "ManageRoles"))
                    throw new UnauthorizedAccessException("You do not have permission to delete roles.");

                using (var context = new HumanResourcesDbContext())
                {
                    using var transaction = context.Database.BeginTransaction();

                    var existing = context.Roles.Include(r => r.RolePermissions).FirstOrDefault(r => r.RoleId == role.RoleId);
                    if (existing != null)
                    {
                        context.RolePermissions.RemoveRange(existing.RolePermissions);
                        context.Roles.Remove(existing);
                        context.SaveChanges();
                        transaction.Commit();

                        LogOperation(user, "Delete", nameof(Role), existing.RoleId, existing, null);
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(user, "DeleteRole", ex);
            }
        }

        // User Delete Operations
        public void DeleteUser(User user, User targetUser)
        {
            try
            {
                if (!HasPermission(user, "ManageUsers"))
                    throw new UnauthorizedAccessException("You do not have permission to delete users.");

                using (var context = new HumanResourcesDbContext())
                {
                    using var transaction = context.Database.BeginTransaction();

                    var existing = context.Users.Find(targetUser.UserId);
                    if (existing != null)
                    {
                        context.SystemLogs.Where(log => log.UserId == targetUser.UserId).ToList().ForEach(log => log.UserId = null);
                        context.EmployeePayrolls.Where(p => p.ProcessedBy == targetUser.UserId).ToList().ForEach(p => p.ProcessedBy = null);
                        context.Employees.Where(e => e.UserId == targetUser.UserId).ToList().ForEach(e => e.UserId = null);

                        context.Users.Remove(existing);
                        context.SaveChanges();
                        transaction.Commit();

                        LogOperation(user, "Delete", nameof(User), existing.UserId, existing, null);
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(user, "DeleteUser", ex);
            }
        }

        // PayPeriod Delete Operations
        public void DeletePayPeriod(User user, PayPeriod period)
        {
            try
            {
                if (!HasPermission(user, "ProcessPayroll"))
                    throw new UnauthorizedAccessException("You do not have permission to delete pay periods.");

                using (var context = new HumanResourcesDbContext())
                {
                    using var transaction = context.Database.BeginTransaction();

                    var existing = context.PayPeriods.Find(period.PayPeriodId);
                    if (existing != null && CanDeletePayPeriod(existing))
                    {
                        var payrolls = context.EmployeePayrolls.Where(p => p.PayPeriodId == existing.PayPeriodId).ToList();
                        payrolls.ForEach(p => context.RemoveRange(p.PayrollDetails));
                        context.EmployeePayrolls.RemoveRange(payrolls);

                        context.PayPeriods.Remove(existing);
                        context.SaveChanges();
                        transaction.Commit();

                        LogOperation(user, "Delete", nameof(PayPeriod), existing.PayPeriodId, existing, null);
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(user, "DeletePayPeriod", ex);
            }
        }

        // PerformanceCriteria Delete Operations
        public void DeletePerformanceCriteria(User user, PerformanceCriterion criterion)
        {
            try
            {
                if (!HasPermission(user, "ManagePerformance"))
                    throw new UnauthorizedAccessException("You do not have permission to delete performance criteria.");

                var existing = _context.PerformanceCriteria.Find(criterion.CriteriaId);
                if (existing != null && !IsPerformanceCriterionUsedInScores(existing.CriteriaId))
                {
                    _context.PerformanceCriteria.Remove(existing);
                    _context.SaveChanges();
                    LogOperation(user, "Delete", nameof(PerformanceCriterion), existing.CriteriaId, existing, null);
                }
            }
            catch (Exception ex)
            {
                LogError(user, "DeletePerformanceCriteria", ex);
            }
        }

        // PerformanceReview Delete Operations
        public void DeletePerformanceReview(User user, PerformanceReview review)
        {
            try
            {
                if (!HasPermission(user, "ManagePerformance"))
                    throw new UnauthorizedAccessException("You do not have permission to delete performance reviews.");

                using (var context = new HumanResourcesDbContext())
                {
                    using var transaction = context.Database.BeginTransaction();

                    var existing = context.PerformanceReviews.Include(r => r.PerformanceScores).FirstOrDefault(r => r.ReviewId == review.ReviewId);
                    if (existing != null)
                    {
                        context.PerformanceScores.RemoveRange(existing.PerformanceScores);
                        context.PerformanceReviews.Remove(existing);
                        context.SaveChanges();
                        transaction.Commit();

                        LogOperation(user, "Delete", nameof(PerformanceReview), existing.ReviewId, existing, null);
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(user, "DeletePerformanceReview", ex);
            }
        }

        // PayrollItems Delete Operations
        public void DeletePayrollItem(User user, PayrollItem item)
        {
            try
            {
                if (!HasPermission(user, "ProcessPayroll"))
                    throw new UnauthorizedAccessException("You do not have permission to delete payroll items.");

                var existing = _context.PayrollItems.Find(item.PayrollItemId);
                if (existing != null)
                {
                    _context.PayrollItems.Remove(existing);
                    _context.SaveChanges();
                    LogOperation(user, "Delete", nameof(PayrollItem), existing.PayrollItemId, existing, null);
                }
            }
            catch (Exception ex)
            {
                LogError(user, "DeletePayrollItem", ex);
            }
        }
    }
}