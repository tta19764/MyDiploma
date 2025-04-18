using HumanResourcesApp.Classes;
using HumanResourcesApp.Models;
using HumanResourcesApp.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user != null && Login.VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }
            else
            {
                return null;
            }
        }

        // Role Read Operations
        public List<Role> GetAllRoles()
        {
            return _context.Roles.AsNoTracking().ToList();
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
            if (_context.Departments.Count() == 0)
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
            if (_context.Positions.Count() == 0)
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
            if (_context.TimeOffTypes.Count() == 0)
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
            _context.Employees.Add(employee);
            _context.SaveChanges();
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
    }
}