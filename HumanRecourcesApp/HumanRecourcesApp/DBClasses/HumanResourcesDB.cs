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

        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
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

        public void UpdateLastLogin(int userId)
        {
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                user.LastLogin = DateTime.Now;
                _context.SaveChanges();
            }
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public List<Role> GetAllRoles()
        {
            return _context.Roles.ToList();
        }

        public Role? GetRoleById(int id)
        {
            return _context.Roles.AsNoTracking().FirstOrDefault(s => s.RoleId == id);
        }

        public void UpdateRole(User user, Role role)
        {
            user.Role = role;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public List<Department> GetAllDepartments()
        {
            return _context.Departments.ToList();
        }

        public async Task<List<Department>> GetAllDepartmentsAsync()
        {
            return await _context.Departments.ToListAsync();
        }

        public List<Employee> GetAllEmployees()
        {
            return _context.Employees.AsNoTracking().ToList();
        }

        public List<Employee> GetAllEmplyeesByPosition(Position position)
        {
            return _context.Employees.AsNoTracking().Where(s => s.Position == position).ToList();
        }

        public List<Employee> GetAllFreeEmplyeesByPosition(Position position)
        {
            return _context.Employees.AsNoTracking().Where(s => s.Position == position && s.Departments.Count() == 0).ToList();
        }

        public Employee? GetEmployeeById(int id)
        {
            return _context.Employees.AsNoTracking().FirstOrDefault(s => s.EmployeeId == id);
        }

        public void DeleteEmployee(Employee employee)
        {
            var existingEmployee = _context.Employees.Find(employee.EmployeeId);
            if (existingEmployee != null)
            {
                _context.Employees.Remove(existingEmployee);
                _context.SaveChanges();
            }
        }

        public void AddEmployee(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
        }

        public void UpdateEmployee(Employee employee)
        {
            _context.Entry(employee).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void AddDepartment(Department department)
        {
            _context.Departments.Add(department);
            _context.SaveChanges();
        }

        public void UpdateDepartment(Department department)
        {
            _context.Entry(department).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public bool IsUniqueDepartmentName(Department department)
        {
            if(_context.Departments.Count() == 0)
            {
                return true;
            }
            else if(department.DepartmentId == 0)
            {
                return !_context.Departments.AsNoTracking().Any(d => d.DepartmentName.ToLower() == department.DepartmentName.Trim().ToLower());
            }
            else
            {
                return !_context.Departments.AsNoTracking().Any(d => d.DepartmentName.ToLower() == department.DepartmentName.Trim().ToLower() && d.DepartmentId != department.DepartmentId);
            }
        }

        public void DeleteDepartment(Department department)
        {
            var existingDepartment = _context.Departments.Find(department.DepartmentId);
            if (existingDepartment != null)
            {
                _context.Departments.Remove(existingDepartment);
                _context.SaveChanges();
            }
        }

        public List<Position> GetAllPositions()
        {
            return _context.Positions.ToList();
        }

        public Position? GetPositionById(int id)
        {
            return _context.Positions.AsNoTracking().FirstOrDefault(s => s.PositionId == id);
        }

        public void AddPosition(Position position)
        {
            _context.Positions.Add(position);
            _context.SaveChanges();
        }

        public void DeletePosition(Position position)
        {
            var existingPosition = _context.Positions.Find(position.PositionId);
            if (existingPosition != null)
            {
                _context.Positions.Remove(existingPosition);
                _context.SaveChanges();
            }
        }

        public void UpdatePosition(Position position)
        {
            _context.Entry(position).State = EntityState.Modified;
            _context.SaveChanges();
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
    }
}
