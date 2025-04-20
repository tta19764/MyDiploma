using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResourcesApp.Models
{
    public partial class TimeOffTypeDisplayModel : ObservableObject
    {
        public int TimeOffTypeId { get; set; }

        [ObservableProperty]
        private bool isEdditing;

        public string DefaultPeriod { get; set; } = null!;

        public string TimeOffTypeName { get; set; } = null!;

        public string? Description { get; set; }

        public int? DefaultDays { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual ICollection<TimeOffBalance> TimeOffBalances { get; set; } = new List<TimeOffBalance>();

        public virtual ICollection<TimeOffRequest> TimeOffRequests { get; set; } = new List<TimeOffRequest>();
    }
}
