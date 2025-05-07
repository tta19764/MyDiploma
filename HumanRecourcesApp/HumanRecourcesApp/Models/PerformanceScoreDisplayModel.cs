using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HumanResourcesApp.Models
{
    public partial class PerformanceScoreViewModel : ObservableObject
    {
        public int ScoreId { get; set; }
        public int ReviewId { get; set; }
        public int CriteriaId { get; set; }

        [ObservableProperty]
        private decimal score;

        [ObservableProperty]
        private string comments = string.Empty;

        public PerformanceCriterion Criteria { get; set; } = new PerformanceCriterion();

        public PerformanceScore ToModel()
        {
            return new PerformanceScore
            {
                ScoreId = ScoreId,
                ReviewId = ReviewId,
                CriteriaId = CriteriaId,
                Score = Score,
                Comments = Comments,
                CreatedAt = DateTime.Now
            };
        }
    }
}
