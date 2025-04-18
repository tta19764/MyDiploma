using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HumanResourcesApp.Models;
using CommunityToolkit.Mvvm.Input;
using HumanResourcesApp.DBClasses;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace HumanResourcesApp.ViewModels
{
    public partial class TimeOffTypesViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;

        [ObservableProperty]
        private ObservableCollection<TimeOffTypeDisplayModel> timeOffTypes;

        [ObservableProperty]
        private TimeOffTypeDisplayModel selectedTimeOffType;

        [ObservableProperty]
        private bool isAddingNew;

        [ObservableProperty]
        private TimeOffTypeDisplayModel newTimeOffType;

        public TimeOffTypesViewModel()
        {
            _context = new HumanResourcesDB();

            TimeOffTypes = new ObservableCollection<TimeOffTypeDisplayModel>();

            // Load data when the view model is created
            LoadTimeOffTypes();
        }


        private void LoadTimeOffTypes()
        {
            try
            {
                // Get all time off types from the database
                var types = _context.GetAllTimeOffTypes();

                TimeOffTypes.Clear();
                foreach (var type in types)
                {
                    TimeOffTypes.Add(
                        new TimeOffTypeDisplayModel
                        {
                            TimeOffTypeId = type.TimeOffTypeId,
                            TimeOffTypeName = type.TimeOffTypeName,
                            Description = type.Description,
                            DefaultDays = type.DefaultDays,
                            IsActive = type.IsActive,
                            CreatedAt = type.CreatedAt,
                            IsEdditing = false
                        }
                        );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                        ex.Message,
                        "Error loading time off types",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
            }
        }

        [RelayCommand]
        private void AddTimeOffType()
        {
            NewTimeOffType = new TimeOffTypeDisplayModel
            {
                TimeOffTypeName = string.Empty,
                Description = string.Empty,
                DefaultDays = 0,
                IsActive = true,
                CreatedAt = DateTime.Now,
                IsEdditing = false
            };
            IsAddingNew = true;
        }

        [RelayCommand]
        private void EdditTimeOffType(TimeOffTypeDisplayModel timeOffType)
        {
            if (timeOffType == null) return;

            SelectedTimeOffType = timeOffType;
            SelectedTimeOffType.IsEdditing = true;
        }

        [RelayCommand]
        private void Save()
        {
            try
            {
                if (IsAddingNew)
                {
                    if(string.IsNullOrWhiteSpace(NewTimeOffType.TimeOffTypeName))
                    {
                        MessageBox.Show("Time Off Type Name is required.");
                        return;
                    }

                    // Save new time off type
                    var newTimeOffType = new TimeOffType
                    {
                        TimeOffTypeName = NewTimeOffType.TimeOffTypeName,
                        Description = NewTimeOffType.Description,
                        DefaultDays = NewTimeOffType.DefaultDays,
                        IsActive = NewTimeOffType.IsActive,
                        CreatedAt = DateTime.Now
                    };

                    if (!_context.IsUniqueTimeOffTypeName(newTimeOffType))
                    {
                        MessageBox.Show("Time Off Type Name must be unique.");
                        return;
                    }

                    _context.AddTimeOffType(newTimeOffType);

                    // Add to collection
                    TimeOffTypes.Add(NewTimeOffType);

                    IsAddingNew = false;
                }
                else
                {
                    // Update existing time off type
                    var existingType = _context.GetTimeOffTypeById(SelectedTimeOffType.TimeOffTypeId);
                    if (existingType != null)
                    {
                        if (string.IsNullOrWhiteSpace(SelectedTimeOffType.TimeOffTypeName))
                        {
                            MessageBox.Show("Time Off Type Name is required.");
                            return;
                        }
                        
                        // Update properties
                        existingType.TimeOffTypeName = SelectedTimeOffType.TimeOffTypeName;
                        existingType.Description = SelectedTimeOffType.Description;
                        existingType.DefaultDays = SelectedTimeOffType.DefaultDays;
                        existingType.IsActive = SelectedTimeOffType.IsActive;

                        if (!_context.IsUniqueTimeOffTypeName(existingType))
                        {
                            MessageBox.Show("Time Off Type Name must be unique.");
                            return;
                        }

                        _context.UpdateTimeOffType(existingType);

                        // Update in collection
                        var index = TimeOffTypes.IndexOf(TimeOffTypes.FirstOrDefault(t => t.TimeOffTypeId == existingType.TimeOffTypeId));
                        if (index >= 0)
                        {
                            TimeOffTypes[index] = new TimeOffTypeDisplayModel()
                            {
                                TimeOffTypeId = existingType.TimeOffTypeId,
                                TimeOffTypeName = existingType.TimeOffTypeName,
                                Description = existingType.Description,
                                DefaultDays = existingType.DefaultDays,
                                IsActive = existingType.IsActive,
                                CreatedAt = existingType.CreatedAt,
                                IsEdditing = false
                            };
                        }
                    }
                }
                
                NewTimeOffType = null;
                SelectedTimeOffType = null; // Clear selection
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                            ex.Message,
                            "Error saving time off type",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            if (IsAddingNew)
            {
                IsAddingNew = false;
                NewTimeOffType = new TimeOffTypeDisplayModel() { IsEdditing = false };
            }
            else if (SelectedTimeOffType != null && SelectedTimeOffType.IsEdditing)
            {
                // Reload the original data for the selected item
                var originalTimeOffType = _context.GetTimeOffTypeById(SelectedTimeOffType.TimeOffTypeId);
                if (originalTimeOffType != null)
                {
                    // Restore original values
                    int index = TimeOffTypes.IndexOf(SelectedTimeOffType);
                    TimeOffTypes[index] = new TimeOffTypeDisplayModel() { 
                        TimeOffTypeId = originalTimeOffType.TimeOffTypeId, 
                        DefaultDays = originalTimeOffType.DefaultDays, 
                        Description = originalTimeOffType.Description,
                        TimeOffTypeName = originalTimeOffType.TimeOffTypeName,
                        IsActive = originalTimeOffType.IsActive,
                        CreatedAt = originalTimeOffType.CreatedAt
                    };
                    SelectedTimeOffType = TimeOffTypes[index];
                }

                SelectedTimeOffType.IsEdditing = false;
            }
        }

        [RelayCommand]
        private void DeleteTimeOffType(TimeOffTypeDisplayModel timeOffType)
        {
            if (timeOffType == null) return;

            try
            {
                // Delete from database
                var typeToDelete = _context.GetTimeOffTypeById(timeOffType.TimeOffTypeId);
                if (typeToDelete != null)
                {
                    // Check if the time off type is used in any time off requests
                    if (_context.IsTimeOffTypeUsedInRequests(typeToDelete))
                    {
                        MessageBox.Show(
                            "Cannot delete this time off type as it is used in existing time off requests.",
                            "Error deleting time off type",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }

                    _context.DeleteTimeOffType(typeToDelete);

                    // Remove from collection
                    TimeOffTypes.Remove(timeOffType);

                    // Clear selection if deleted item was selected
                    if (SelectedTimeOffType == timeOffType)
                    {
                        SelectedTimeOffType = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                        ex.Message,
                        "Error deleting time off type",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
            }
            finally
            {
                LoadTimeOffTypes();
            }
        }
    }
}