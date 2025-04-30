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
    public partial class PerformanceReviewDetailViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;

        #region Properties
        [ObservableProperty]
        private PerformanceReview review;

        [ObservableProperty]
        private string employeeName;

        [ObservableProperty]
        private string reviewerName;

        [ObservableProperty]
        private ObservableCollection<PerformanceScore> performanceScores;

        [ObservableProperty]
        private List<string> reviewStatuses;

        [ObservableProperty]
        private string selectedStatus;

        [ObservableProperty]
        private bool showAcknowledgement;

        public event EventHandler<bool> RequestClose;
        #endregion

        public PerformanceReviewDetailViewModel(PerformanceReview review)
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

            // Initialize status options based on current status
            InitializeStatusOptions();

            // Determine if acknowledgement should be shown
            // Show acknowledgement option if the review is submitted and not yet acknowledged
            ShowAcknowledgement = Review.Status == "Submitted";
        }

        private void InitializeStatusOptions()
        {
            switch (Review.Status)
            {
                case "Pending":
                    ReviewStatuses = new List<string> { "Pending", "Submitted" };
                    break;
                case "Submitted":
                    ReviewStatuses = new List<string> { "Submitted", "Acknowledged" };
                    break;
                case "Acknowledged":
                    ReviewStatuses = new List<string> { "Acknowledged" };
                    break;
                default:
                    ReviewStatuses = new List<string> { Review.Status };
                    break;
            }

            SelectedStatus = Review.Status;
        }

        [RelayCommand]
        private void Save()
        {
            try
            {
                // Update review status if changed
                bool statusChanged = Review.Status != SelectedStatus;
                Review.Status = SelectedStatus;

                // Update submission date if status changed to Submitted
                if (statusChanged && SelectedStatus == "Submitted")
                {
                    Review.SubmissionDate = DateTime.Now;
                }

                // Update scores
                foreach (var score in PerformanceScores)
                {
                    var existingScore = _context.GetPerformanceScoreById(score.ScoreId);
                    if (existingScore != null)
                    {
                      existingScore.Score = score.Score;
                      existingScore.Comments = score.Comments;
                      _context.UpdatePerformanceScore(existingScore);
                    }
                }

                _context.UpdatePerformanceReview(Review);

                MessageBox.Show("Performance review updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                RequestClose?.Invoke(this, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating performance review: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Acknowledge()
        {
            try
            {
                // Update review status and acknowledgement date
                Review.Status = "Acknowledged";
                Review.AcknowledgementDate = DateTime.Now;

                // Update in database
                _context.UpdatePerformanceReview(Review);

                MessageBox.Show("Performance review acknowledged successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                RequestClose?.Invoke(this, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error acknowledging performance review: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            RequestClose?.Invoke(this, false);
        }
    }
}