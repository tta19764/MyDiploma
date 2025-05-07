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
        [ObservableProperty] private string reviewerName = string.Empty;
        [ObservableProperty] private string reviewPeriod = string.Empty;
        [ObservableProperty] private DateOnly reviewDate;
        [ObservableProperty] private decimal? overallRating;
        [ObservableProperty] private string comments = string.Empty;
        [ObservableProperty] private string status = string.Empty;

        public DateTime? SubmissionDate { get; set; }

        [ObservableProperty] public DateTime? acknowledgementDate;

        public DateTime? CreatedAt { get; set; }
    }

}
