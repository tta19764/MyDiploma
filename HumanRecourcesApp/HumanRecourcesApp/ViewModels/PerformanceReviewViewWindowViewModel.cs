using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumanResourcesApp.DBClasses;
using HumanResourcesApp.Models;
using HumanResourcesApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace HumanResourcesApp.ViewModels
{
    public partial class PerformanceReviewViewWindowViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;

        [ObservableProperty]
        private PerformanceReview review;

        [ObservableProperty]
        private string employeeName;

        [ObservableProperty]
        private string reviewerName;

        [ObservableProperty]
        private ObservableCollection<PerformanceScore> performanceScores;

        public PerformanceReviewViewWindowViewModel(PerformanceReview review)
        {
            _context = new HumanResourcesDB();
            Review = review;
            EmployeeName = $"{review.Employee.FirstName} {review.Employee.LastName}";
            ReviewerName = $"{review.Reviewer.FirstName} {review.Reviewer.LastName}";

            // Deep copy of performance scores to allow editing
            PerformanceScores = new ObservableCollection<PerformanceScore>(
                review.PerformanceScores.Select(ps => new PerformanceScore
                {
                    ScoreId = ps.ScoreId,
                    ReviewId = ps.ReviewId,
                    CriteriaId = ps.CriteriaId,
                    Score = ps.Score,
                    Comments = ps.Comments,
                    CreatedAt = ps.CreatedAt,
                    Criteria = ps.Criteria
                })
            );
        }

        [RelayCommand]
        private void Close()
        {
            if (Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this) is Window window)
            {
                window.Close();
            }
        }
    }
}