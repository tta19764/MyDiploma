using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResourcesApp.Models
{
    public class TimeOffRequestDisplayModel
    {
        public int TimeOffRequestId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string TimeOffTypeName { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int TotalDays { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string? Comments { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovalDate { get; set; }
    }
}
