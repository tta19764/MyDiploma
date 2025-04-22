using HumanResourcesApp.Classes;
using HumanResourcesApp.Models;
using HumanResourcesApp.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public List<Employee> GetEmployeesByPosition(Position position)
        {
            return _context.Employees.AsNoTracking().Where(s => s.Position == position).ToList();
        }

        public List<Employee> GetEmployeesByDepartment(Department department)
        {
            return _context.Employees.AsNoTracking().Where(s => s.Department == department).ToList();
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

        // Attendance Read Operations
        public List<Attendance> GetAllAttendances()
        {
            return _context.Attendances.AsNoTracking().ToList();
        }

        public Attendance? GetAttendanceById(int id)
        {
            return _context.Attendances.AsNoTracking().FirstOrDefault(s => s.AttendanceId == id);
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

        // Permission Read Operations
        public List<Permission> GetAllPermissions()
        {
            return _context.Permissions.AsNoTracking().ToList();
        }

        // =====================================
        // CREATE OPERATIONS
        // =====================================

        // User Create Operations
        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        // Department Create Operations
        public void AddDepartment(Department department)
        {
            _context.Departments.Add(department);
            _context.SaveChanges();
        }

        // Employee Create Operations
        public void AddEmployee(Employee employee)
        {
            try
            {
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
                    var period = (context.TimeOffBalances.FirstOrDefault(s => s.TimeOffTypeId == type.TimeOffTypeId).Period) ?? "Yearly";

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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding employee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Position Create Operations
        public void AddPosition(Position position)
        {
            _context.Positions.Add(position);
            _context.SaveChanges();
        }

        // Attendance Create Operations
        public void AddAttendance(Attendance attendance)
        {
            _context.Attendances.Add(attendance);
            _context.SaveChanges();
        }

        // TimeOffTypes Create Operations
        public void AddTimeOffType(TimeOffType timeOffType)
        {
            _context.TimeOffTypes.Add(timeOffType);
            _context.SaveChanges();
        }

        // TimeOffRequests Create Operations
        public void AddTimeOffRequest(TimeOffRequest timeOffRequest)
        {
            _context.TimeOffRequests.Add(timeOffRequest);
            _context.SaveChanges();
        }

        // TimeOffBalance Create Operations
        public void CreateTimeOffBalanceByTimeOffType(TimeOffType timeOffType, string period)
        {
            var employees = _context.Employees.AsNoTracking().ToList();
            foreach (var employee in employees)
            {
                var timeOffBalance = new TimeOffBalance
                {
                    EmployeeId = employee.EmployeeId,
                    TimeOffTypeId = timeOffType.TimeOffTypeId,
                    Period = period,
                    TotalDays = timeOffType.DefaultDays ?? 0,
                    UsedDays = 0,
                    RemainingDays = timeOffType.DefaultDays ?? 0
                };
                _context.TimeOffBalances.Add(timeOffBalance);
            }
            _context.SaveChanges();
        }

        //Role Create Operations
        public void CreateRole(Role role)
        {
            _context.Roles.Add(role);
            _context.SaveChanges();
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
            var existingEmployee = _context.Employees.Find(employee.EmployeeId);
            if (existingEmployee != null)
            {
                _context.Employees.Remove(existingEmployee);
                _context.SaveChanges();
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
    }
}