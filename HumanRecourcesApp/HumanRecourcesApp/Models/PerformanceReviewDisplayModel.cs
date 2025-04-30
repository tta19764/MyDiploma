using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResourcesApp.Models
{
    public partial class PerformanceReviewDisplayModel : ObservableObject
    {
        public int ReviewId { get; set; }
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; } = string.Empty;

        [ObservableProperty] private int reviewerId;
        [ObservableProperty] private string reviewerName;
        [ObservableProperty] private string reviewPeriod;
        [ObservableProperty] private DateOnly reviewDate;
        [ObservableProperty] private decimal? overallRating;
        [ObservableProperty] private string comments;
        [ObservableProperty] private string status;

        public DateTime? SubmissionDate { get; set; }

        [ObservableProperty] public DateTime? acknowledgementDate;

        public DateTime? CreatedAt { get; set; }
    }

}
