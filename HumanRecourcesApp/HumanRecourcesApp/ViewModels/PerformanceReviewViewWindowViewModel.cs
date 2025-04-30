using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumanResourcesApp.DBClasses;
using HumanResourcesApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace HumanResourcesApp.ViewModels
{
    public partial class PerformanceReviewViewWindowViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;

        public event EventHandler<bool> RequestClose;

        #region Properties
        [ObservableProperty]
        private PerformanceReview review;

        [ObservableProperty]
        private ObservableCollection<EmployeeDisplayModel> employees;

        [ObservableProperty]
        private ObservableCollection<EmployeeDisplayModel> reviewers;

        [ObservableProperty]
        private ObservableCollection<PerformanceScoreViewModel> performanceScores;

        [ObservableProperty]
        private DateTime reviewDate;

        public List<decimal> RatingOptions => new() { 1.0m, 1.5m, 2.0m, 2.5m, 3.0m, 3.5m, 4.0m, 4.5m, 5.0m };

        public List<decimal> ScoreOptions => new() { 1.0m, 1.5m, 2.0m, 2.5m, 3.0m, 3.5m, 4.0m, 4.5m, 5.0m };

        public List<string> StatusOptions => new() { "Draft", "Pending", "Submitted" };
        #endregion

        public PerformanceReviewViewWindowViewModel()
        {
            _context = new HumanResourcesDB();
            InitializeNew();
        }

        public PerformanceReviewViewWindowViewModel(PerformanceReview existingReview)
        {
            _context = new HumanResourcesDB();
            InitializeWithExisting(existingReview);
        }

        private void InitializeNew()
        {
            // Create new review
            Review = new PerformanceReview
            {
                ReviewDate = DateOnly.FromDateTime(DateTime.Today),
                Status = "Draft",
                CreatedAt = DateTime.Now
            };

            // Set ReviewDate property for DatePicker binding
            ReviewDate = DateTime.Today;

            LoadEmployees();
            LoadCriteria();
        }

        private void InitializeWithExisting(PerformanceReview existingReview)
        {
            Review = existingReview;
            ReviewDate = existingReview.ReviewDate.ToDateTime(TimeOnly.MinValue);

            LoadEmployees();

            // Load existing performance scores
            PerformanceScores = new ObservableCollection<PerformanceScoreViewModel>();

            //var existingScores = _context.GetPerformanceScoresByReviewId(existingReview.ReviewId);
            //var allCriteria = _context.GetAllPerformanceCriteria();

            //// Add existing scores
            //foreach (var score in existingScores)
            //{
                //PerformanceScores.Add(new PerformanceScoreViewModel
                //{
                    //ScoreId = score.ScoreId,
                    //ReviewId = score.ReviewId,
                    //CriteriaId = score.CriteriaId,
                    //Score = score.Score,
                    //Comments = score.Comments,
                    //Criteria = score.Criteria
                //});
            //}

            //// Add missing criteria with default scores
            //foreach (var criteria in allCriteria.Where(c => !existingScores.Any(s => s.CriteriaId == c.CriteriaId)))
            //{
                //PerformanceScores.Add(new PerformanceScoreViewModel
                //{
                    //ReviewId = existingReview.ReviewId,
                    //CriteriaId = criteria.CriteriaId,
                    //Score = 3.0m, // Default score
                    //Criteria = criteria
                //});
            //}
        }

        private void LoadEmployees()
        {
            var allEmployees = _context.GetAllEmplyees();

            Employees = new ObservableCollection<EmployeeDisplayModel>(
                allEmployees.Select(e => new EmployeeDisplayModel
                {
                    EmployeeId = e.EmployeeId,
                    FirstName = e.FirstName,
                    LastName = e.LastName
                })
            );

            Reviewers = new ObservableCollection<EmployeeDisplayModel>(Employees);
        }

        private void LoadCriteria()
        {
            List<PerformanceCriterion> criteria = _context.GetAllPerformanceCriterias();

            PerformanceScores = new ObservableCollection<PerformanceScoreViewModel>(
                criteria.Select(c => new PerformanceScoreViewModel
                {
                    CriteriaId = c.CriteriaId,
                    Score = 3.0m, // Default score
                    Criteria = c
                })
            );
        }

        [RelayCommand]
        private void Save()
        {
            if (!ValidateForm())
                return;

            try
            {
                // Update Review with form values
                //Review.ReviewDate = DateOnly.FromDateTime(ReviewDate);

                //bool isNew = Review.ReviewId == 0;

                //if (isNew)
                //{
                //    // Create new review
                //    int reviewId = _context.CreatePerformanceReview(Review);
                //    Review.ReviewId = reviewId;

                //    // Create all performance scores
                //    foreach (var score in PerformanceScores)
                //    {
                //        score.ReviewId = reviewId;
                //        _context.CreatePerformanceScore(score.ToModel());
                //    }
                //}
                //else
                //{
                //    // Update existing review
                //    _context.UpdatePerformanceReview(Review);

                //    // Update or create performance scores
                //    foreach (var score in PerformanceScores)
                //    {
                //        if (score.ScoreId > 0)
                //            _context.UpdatePerformanceScore(score.ToModel());
                //        else
                //        {
                //            score.ReviewId = Review.ReviewId;
                //            _context.CreatePerformanceScore(score.ToModel());
                //        }
                //    }
                //}

                RequestClose?.Invoke(this, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving performance review: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            RequestClose?.Invoke(this, false);
        }

        private bool ValidateForm()
        {
            if (Review.EmployeeId == 0)
            {
                MessageBox.Show("Please select an employee.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (Review.ReviewerId == 0)
            {
                MessageBox.Show("Please select a reviewer.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Review.ReviewPeriod))
            {
                MessageBox.Show("Please enter a review period.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}