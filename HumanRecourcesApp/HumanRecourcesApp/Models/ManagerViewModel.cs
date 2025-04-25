using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResourcesApp.Models
{
    public class ManagerViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public Position Position { get; set; } = null!;
    }
}
