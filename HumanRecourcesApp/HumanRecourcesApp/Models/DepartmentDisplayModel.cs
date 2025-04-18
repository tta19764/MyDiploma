using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResourcesApp.Models
{
    public class DepartmentDisplayModel
    {
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = null!;

        public string? Description { get; set; }

        public int? ManagerId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

        public string? ManagerFullName { get; set; }
    }
}
