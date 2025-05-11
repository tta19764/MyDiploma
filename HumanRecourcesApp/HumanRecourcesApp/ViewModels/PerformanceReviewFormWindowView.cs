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
    public partial class PerformanceReviewFormViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;
        private readonly User user;
        [ObservableProperty] private ObservableCollection<EmployeeDisplayModel> employees;
        [ObservableProperty] private EmployeeDisplayModel selectedEmployee = new EmployeeDisplayModel();
        [ObservableProperty] private EmployeeDisplayModel selectedReviewer = new EmployeeDisplayModel();
        [ObservableProperty] private string reviewPeriod = string.Empty;
        [ObservableProperty] private DateTime reviewDate;
        [ObservableProperty] private decimal overallRating = 3.0m;
        [ObservableProperty] private string comments = string.Empty;
        [ObservableProperty] private ObservableCollection<PerformanceCriteriaDisplayModel> performanceCriteria = new ObservableCollection<PerformanceCriteriaDisplayModel>();
        [ObservableProperty] private List<string> reviewStatuses;
        [ObservableProperty] private string selectedStatus;

        public PerformanceReviewFormViewModel(User _user)
        {
            _context = new HumanResourcesDB();
            user = _user;
            ReviewDate = DateTime.Now;

            // Load employees
            var employeeList = _context.GetAllEmplyees();
            Employees = new ObservableCollection<EmployeeDisplayModel>();
            foreach (var employee in employeeList)
            {
                Employees.Add(new EmployeeDisplayModel
                {
                    EmployeeId = employee.EmployeeId,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Position = employee.Position
                });
            }

            // Load performance criteria
            LoadPerformanceCriteria();

            // Initialize review statuses
            ReviewStatuses = new List<string> { "Pending", "Submitted" };
            SelectedStatus = "Pending";
        }

        private void LoadPerformanceCriteria()
        {
            var criteria = _context.GetAllActiveCriteria();
            PerformanceCriteria = new ObservableCollection<PerformanceCriteriaDisplayModel>(
                criteria.Select(c => new PerformanceCriteriaDisplayModel
                {
                    CriteriaId = c.CriteriaId,
                    CriteriaName = c.CriteriaName,
                    Description = c.Description ?? string.Empty,
                    Category = c.Category ?? string.Empty,
                    WeightPercentage = c.WeightPercentage ?? 0,
                    Score = 3.0m,
                    Comments = string.Empty
                })
            );
        }

        [RelayCommand]
        private void Save()
        {
            // Validate inputs
            if (SelectedEmployee == null)
            {
                MessageBox.Show("Please select an employee.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedReviewer == null)
            {
                MessageBox.Show("Please select a reviewer.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(ReviewPeriod))
            {
                MessageBox.Show("Please enter a review period.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Create new performance review
            var newReview = new PerformanceReview
            {
                EmployeeId = SelectedEmployee.EmployeeId,
                ReviewerId = SelectedReviewer.EmployeeId,
                ReviewPeriod = ReviewPeriod,
                ReviewDate = DateOnly.FromDateTime(ReviewDate),
                OverallRating = OverallRating,
                Comments = Comments,
                Status = SelectedStatus,
                SubmissionDate = SelectedStatus == "Submitted" ? DateTime.Now : null,
                CreatedAt = DateTime.Now
            };

            // Add performance scores
            foreach (var criteriaScore in PerformanceCriteria)
            {
                var score = new PerformanceScore
                {
                    CriteriaId = criteriaScore.CriteriaId,
                    Score = criteriaScore.Score,
                    Comments = criteriaScore.Comments,
                    CreatedAt = DateTime.Now
                };

                newReview.PerformanceScores.Add(score);
            }

            try
            {
                // Save to database
                _context.CreatePerformanceReview(user, newReview);

                MessageBox.Show("Performance review saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseDialogWithResult(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving performance review: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseDialogWithResult(false);
        }

        private void CloseDialogWithResult(bool? result)
        {
            if (Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this) is Window window)
            {
                window.DialogResult = result;
                window.Close();
            }
        }
    }
}