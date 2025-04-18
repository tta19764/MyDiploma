using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResourcesApp.Models
{
    public class EmployeeDisplayModel
    {
        public int EmployeeId { get; set; }

        public int? UserId { get; set; }

        public string FullName => $"{FirstName} {LastName}" + (string.IsNullOrEmpty(MiddleName) ? "" : $" {MiddleName}");

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? MiddleName { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public Position Position { get; set; } = null!;
    }
}
