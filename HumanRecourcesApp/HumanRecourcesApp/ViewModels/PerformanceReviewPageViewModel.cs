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
using System.Windows.Data;

namespace HumanResourcesApp.ViewModels
{
    public partial class PerformanceReviewsPageViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;

        [ObservableProperty] private ObservableCollection<PerformanceReviewDisplayModel> performanceReviews;
        [ObservableProperty] private ListCollectionView pendingReviewsView;
        [ObservableProperty] private ListCollectionView acknowledgedReviewsView;
        [ObservableProperty] private bool hasPendingReviews;
        [ObservableProperty] private bool hasAcknowledgedReviews;
        private readonly User user;

        public PerformanceReviewsPageViewModel(User _user)
        {
            _context = new HumanResourcesDB();
            user = _user;
            PerformanceReviews = new ObservableCollection<PerformanceReviewDisplayModel>();
            PendingReviewsView = new ListCollectionView(PerformanceReviews);
            AcknowledgedReviewsView = new ListCollectionView(PerformanceReviews);
            LoadReviews();
        }

        private void LoadReviews()
        {
            var reviews = _context.GetAllPerformanceReviews();

            PerformanceReviews = new ObservableCollection<PerformanceReviewDisplayModel>(
                reviews.Select(r => new PerformanceReviewDisplayModel
                {
                    ReviewId = r.ReviewId,
                    EmployeeId = r.EmployeeId,
                    EmployeeName = $"{r.Employee.FirstName} {r.Employee.LastName}",
                    ReviewerId = r.ReviewerId,
                    ReviewerName = $"{r.Reviewer.FirstName} {r.Reviewer.LastName}",
                    ReviewPeriod = r.ReviewPeriod ?? string.Empty,
                    ReviewDate = r.ReviewDate,
                    OverallRating = r.OverallRating,
                    Comments = r.Comments ?? string.Empty,
                    Status = r.Status,
                    SubmissionDate = r.SubmissionDate,
                    AcknowledgementDate = r.AcknowledgementDate,
                    CreatedAt = r.CreatedAt,
                })
            );

            // Create views for pending and acknowledged reviews
            PendingReviewsView = new ListCollectionView(PerformanceReviews);
            PendingReviewsView.Filter = item => ((PerformanceReviewDisplayModel)item).Status == "Pending" ||
                                              ((PerformanceReviewDisplayModel)item).Status == "Submitted";

            PendingReviewsView.SortDescriptions.Add(
                new System.ComponentModel.SortDescription(
                    nameof(PerformanceReviewDisplayModel.SubmissionDate),
                    System.ComponentModel.ListSortDirection.Descending));

            AcknowledgedReviewsView = new ListCollectionView(PerformanceReviews);
            AcknowledgedReviewsView.Filter = item => ((PerformanceReviewDisplayModel)item).Status == "Acknowledged";

            AcknowledgedReviewsView.SortDescriptions.Add(
                new System.ComponentModel.SortDescription(
                    nameof(PerformanceReviewDisplayModel.AcknowledgementDate),
                    System.ComponentModel.ListSortDirection.Descending));

            HasPendingReviews = PendingReviewsView.Count > 0;
            HasAcknowledgedReviews = AcknowledgedReviewsView.Count > 0;
        }

        [RelayCommand]
        private void Refresh() => LoadReviews();

        [RelayCommand]
        private void Create()
        {
            var formWindow = new PerformanceReviewFormWindow(user);
            if (formWindow.ShowDialog() == true)
                LoadReviews();
        }

        [RelayCommand]
        private void Review(PerformanceReviewDisplayModel reviewModel)
        {
            if (reviewModel == null)
                return;

            var review = _context.GetPerformanceReviewById(reviewModel.ReviewId);
            if (review == null)
            {
                MessageBox.Show("The selected performance review could not be found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var formWindow = new PerformanceReviewDetailWindow();
            var viewModel = new PerformanceReviewDetailViewModel(review);
            formWindow.DataContext = viewModel;

            viewModel.RequestClose += (sender, result) =>
            {
                formWindow.DialogResult = result;
                formWindow.Close();
            };

            bool? result = formWindow.ShowDialog();
            if (result == true)
            {
                LoadReviews();
            }
        }

        [RelayCommand]
        private void View(PerformanceReviewDisplayModel reviewModel)
        {
            if (reviewModel == null)
                return;

            var review = _context.GetPerformanceReviewById(reviewModel.ReviewId);
            if (review == null)
            {
                MessageBox.Show("The selected performance review could not be found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var viewWindow = new PerformanceReviewViewWindow(review);
            viewWindow.ShowDialog();
        }

        [RelayCommand]
        private void Delete(PerformanceReviewDisplayModel reviewModel)
        {
            if (reviewModel == null)
                return;

            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to delete this performance review?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var review = _context.GetPerformanceReviewById(reviewModel.ReviewId);
                if (review != null)
                {
                    _context.DeletePerformanceReview(review);
                    LoadReviews();
                }
            }
        }
    }
}