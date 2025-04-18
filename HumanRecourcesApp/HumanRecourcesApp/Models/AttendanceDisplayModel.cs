using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResourcesApp.Models
{
    public class AttendanceDisplayModel
    {
        public int AttendanceId { get; set; }

        public string EmployeeFullName { get; set; } = null!;

        public int EmployeeId { get; set; }

        public DateTime? CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        public decimal? WorkHours { get; set; }

        public string? Status { get; set; }

        public string? Notes { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual Employee Employee { get; set; } = null!;
    }
}
