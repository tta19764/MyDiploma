using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResourcesApp.Models
{
    public class TimeOffRequestDisplayModel : ObservableObject
    {
        public int TimeOffRequestId { get; set; }

        public string EmployeeName { get; set; } = string.Empty;
        public string TimeOffTypeName { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int TotalDays { get; set; }
        private string status = string.Empty;
        public string Status
        {
            get => status;
            set
            {
                SetProperty(ref status, value);
                OnPropertyChanged(nameof(StatusPriority)); // Notify WPF that StatusPriority also changed!
            }
        }

        public int StatusPriority => Status == "Pending" ? 0 : 1;

        public string? Reason { get; set; }
        public string? Comments { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovalDate { get; set; }
    }
}
