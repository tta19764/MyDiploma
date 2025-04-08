using HumanResourcesApp.Classes;
using HumanResourcesApp.Models;
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
            return _context.Roles.FirstOrDefault(s => s.RoleId == id);
        }

        public void UpdateRole(User user, Role role)
        {
            user.Role = role;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
