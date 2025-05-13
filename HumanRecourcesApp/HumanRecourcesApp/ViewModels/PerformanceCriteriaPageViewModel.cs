using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumanResourcesApp.Classes;
using HumanResourcesApp.DBClasses;
using HumanResourcesApp.Models;
using System.Globalization;

namespace HumanResourcesApp.ViewModels
{
    public partial class PerformanceCriteriaViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;
        private static readonly Regex _numericRegex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");

        [ObservableProperty] private ObservableCollection<PerformanceCriterion> performanceCriteria;
        [ObservableProperty] private PerformanceCriterion selectedCriteria;
        [ObservableProperty] private PerformanceCriterion newCriteria;
        [ObservableProperty] private bool isAddingOrEditing;
        [ObservableProperty] private bool isEditing;
        [ObservableProperty] private string formTitle;
        [ObservableProperty] private string weightPercentageText = "";
        private readonly User user;


        [ObservableProperty] private bool canManagePerformance = false;

        public PerformanceCriteriaViewModel(User _user)
        {
            // Initialize context
            _context = new HumanResourcesDB();
            user = _user;

            // Initialize collections
            PerformanceCriteria = new ObservableCollection<PerformanceCriterion>();
            NewCriteria = new PerformanceCriterion();
            SelectedCriteria = new PerformanceCriterion();

            // Set default form title
            FormTitle = "Add Performance Criteria";


            CanManagePerformance = _context.HasPermission(user, "ManagePerformance");
            // Load data
            LoadPerformanceCriteria();
        }

        private string _lastValidWeightPercentageText = "";

        partial void OnWeightPercentageTextChanged(string value)
        {
            // Only allow numeric values
            if (!string.IsNullOrEmpty(value) && !_numericRegex.IsMatch(value))
            {
                // Revert to the last valid value
                WeightPercentageText = _lastValidWeightPercentageText;
                return;
            }

            // Update the model when valid input is provided
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal weight))
            {
                NewCriteria.WeightPercentage = weight;
                _lastValidWeightPercentageText = value; // Store the valid value
            }
            else
            {
                NewCriteria.WeightPercentage = null;
                if (string.IsNullOrEmpty(value))
                {
                    _lastValidWeightPercentageText = ""; // Reset stored value if field is cleared
                }
            }
        }

        private void LoadPerformanceCriteria()
        {
            PerformanceCriteria.Clear();
            var criteriaList = _context.GetAllPerformanceCriterias();
            foreach (var criteria in criteriaList)
            {
                PerformanceCriteria.Add(criteria);
            }
        }

        [RelayCommand]
        private void AddCriteria()
        {
            IsAddingOrEditing = true;
            IsEditing = false;
            FormTitle = "Add Performance Criteria";
            SelectedCriteria = new PerformanceCriterion();

            // Set default values for new criteria
            NewCriteria = new PerformanceCriterion
            {
                CriteriaName = "",
                Description = "",
                Category = "Productivity",
                WeightPercentage = 10,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            // Set the text representation of the weight percentage
            WeightPercentageText = NewCriteria.WeightPercentage?.ToString() ?? "";
        }

        [RelayCommand]
        private void EditCriteria(PerformanceCriterion criteria)
        {
            try
            {
                if (criteria == null) throw new Exception("Performance criteria not found.");

                IsAddingOrEditing = true;
                IsEditing = true;
                FormTitle = "Edit Performance Criteria";

                // Create a copy for editing
                NewCriteria = new PerformanceCriterion
                {
                    CriteriaId = criteria.CriteriaId,
                    CriteriaName = criteria.CriteriaName,
                    Description = criteria.Description,
                    Category = criteria.Category,
                    WeightPercentage = criteria.WeightPercentage,
                    IsActive = criteria.IsActive,
                    CreatedAt = criteria.CreatedAt
                };

                // Set the text representation of the weight percentage
                WeightPercentageText = NewCriteria.WeightPercentage?.ToString("F2", CultureInfo.InvariantCulture) ?? "";
            }
            catch (Exception ex)
            {
                _context.LogError(user, "EditPerformanceCriteria", ex);
            }
        }

        [RelayCommand]
        private void Save()
        {
            if (!IsAddingOrEditing) return;

            // Validate name
            if (string.IsNullOrWhiteSpace(NewCriteria.CriteriaName))
            {
                MessageBox.Show("Criteria name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate weight percentage formatting
            if (!decimal.TryParse(WeightPercentageText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal weightValue))
            {
                MessageBox.Show("Weight percentage must be a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate weight percentage range
            if (weightValue < 0 || weightValue > 100)
            {
                MessageBox.Show("Weight percentage must be between 0 and 100.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Update the model with the parsed value
            NewCriteria.WeightPercentage = weightValue;

            try
            {
                if (IsEditing)
                {
                    _context.UpdatePerformanceCriterion(user, NewCriteria);
                }
                else
                {
                    _context.CreatePerformanceCriterion(user, NewCriteria);
                }

                LoadPerformanceCriteria();
                IsAddingOrEditing = false;
            }
            catch (Exception ex)
            {
                _context.LogError(user, "SavePerformanceCriterion", ex);
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            IsAddingOrEditing = false;
            NewCriteria = new PerformanceCriterion();
            WeightPercentageText = "";
        }

        [RelayCommand]
        private void DeleteCriteria(PerformanceCriterion criteria)
        {
            try
            {
                if (criteria == null) throw new Exception("Performance criteria not found.");

                // Check if criteria has associated performance scores
                int scoreCount = criteria.PerformanceScores.Count;
                if (scoreCount > 0)
                {
                    var result = MessageBox.Show(
                        $"This performance criteria has {scoreCount} associated performance scores. Deleting it will also delete all related score data.\n\nDo you want to  continue?",
                        "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                else
                {
                    var result = MessageBox.Show($"Are you sure you want to delete the performance criteria '{criteria.CriteriaName}'?",
                        "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }

            
                _context.DeletePerformanceCriteria(user, criteria);
                PerformanceCriteria.Remove(criteria);
            }
            catch (Exception ex)
            {
                _context.LogError(user, "DeletePerformanceCriteria", ex);
            }
        }
    }
}