using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumanResourcesApp.DBClasses;
using HumanResourcesApp.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace HumanResourcesApp.ViewModels
{
    public partial class PositionViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;
        [ObservableProperty] private ObservableCollection<PositionDisplayModel> positions;
        [ObservableProperty] private PositionDisplayModel selectedPosition;
        [ObservableProperty] private PositionDisplayModel newPosition;
        [ObservableProperty] private bool isAddingNew;
        private readonly User user;

        public PositionViewModel(User _user)
        {
            // Initialize commands
            _context = new HumanResourcesDB();

            // Initialize collections
            Positions = new ObservableCollection<PositionDisplayModel>();
            NewPosition = new PositionDisplayModel();
            SelectedPosition = new PositionDisplayModel();
            user = _user;

            // Load data
            LoadPositions();
        }
        private void LoadPositions()
        {
            Positions.Clear();
            var positionsList = _context.GetAllPositions();
            foreach (var position in positionsList)
            {
                Positions.Add(new PositionDisplayModel { PositionId = position.PositionId, CreatedAt = position.CreatedAt, Description = position.Description ?? string.Empty, PositionTitle = position.PositionTitle, IsEdditing = false});
            }
        }

        [RelayCommand]
        private void Add()
        {
            IsAddingNew = true;
            NewPosition = new PositionDisplayModel
            {
                PositionId = 0,
                PositionTitle = string.Empty,
                Description = string.Empty,
                CreatedAt = DateTime.Now,
                IsEdditing = false
            };
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedPosition != null && SelectedPosition.IsEdditing)
            {
                // Validate
                if (string.IsNullOrWhiteSpace(SelectedPosition.PositionTitle))
                {
                    MessageBox.Show("Position Title is required.");
                    return;
                }
                else
                {
                    // Save changes
                    var positionToUpdate = new Position
                    {
                        PositionId = SelectedPosition.PositionId,
                        PositionTitle = SelectedPosition.PositionTitle,
                        Description = SelectedPosition.Description,
                        CreatedAt = SelectedPosition.CreatedAt
                    };

                    if(_context.IsUniquePositionTitle(positionToUpdate))
                    {
                        _context.UpdatePosition(user, positionToUpdate);
                    }
                    else
                    {
                        MessageBox.Show("Position Title must be unique.");
                        return;
                    }
                }


                SelectedPosition.IsEdditing = false;
            }
        }

        [RelayCommand]
        private void SaveNew()
        {
            if (IsAddingNew)
            {
                // Save new position
                if (string.IsNullOrWhiteSpace(NewPosition.PositionTitle))
                {
                    MessageBox.Show("Position Title is required.");
                    return;
                }

                var positionToAdd = new Position
                {
                    PositionTitle = NewPosition.PositionTitle,
                    Description = NewPosition.Description,
                    CreatedAt = NewPosition.CreatedAt
                };

                if (_context.IsUniquePositionTitle(positionToAdd))
                {
                    _context.AddPosition(user, positionToAdd);
                }
                else
                {
                    MessageBox.Show("Position Title must be unique.");
                    return;
                }
                

                LoadPositions();

                // Reset add mode
                IsAddingNew = false;
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            if (IsAddingNew)
            {
                IsAddingNew = false;
                NewPosition = new PositionDisplayModel() { IsEdditing = false };
            }
            else if (SelectedPosition != null && SelectedPosition.IsEdditing)
            {
                // Reload the original data for the selected item
                var originalPosition = _context.GetPositionById(SelectedPosition.PositionId);
                if (originalPosition != null)
                {
                    // Restore original values
                    int index = Positions.IndexOf(SelectedPosition);
                    Positions[index] = new PositionDisplayModel() { 
                        PositionId = originalPosition.PositionId, 
                        CreatedAt = originalPosition.CreatedAt, 
                        Description = originalPosition.Description ?? string.Empty,
                        PositionTitle = originalPosition.PositionTitle
                    };
                    SelectedPosition = Positions[index];
                }

                SelectedPosition.IsEdditing = false;
            }
        }

        [RelayCommand]
        private void EditPosition(PositionDisplayModel position)
        {
            try
            {
                if (position == null) throw new Exception("Position not found.");

                // Store the selected position and enter edit mode
                SelectedPosition = position;
                SelectedPosition.IsEdditing = true;
            }
            catch (Exception ex)
            {
                _context.LogError(user, "EditPosition", ex);
            }
        }

        [RelayCommand]
        private void DeletePosition(PositionDisplayModel position)
        {
            try
            {
                var positionToDelete = _context.GetPositionById(position.PositionId);
                if (positionToDelete == null)
                {
                    throw new Exception("Position not found.");
                }
                else
                {
                    // Check if the position is in use
                    if (_context.IsPositionUsedInEmployees(positionToDelete.PositionId))
                    {
                        MessageBox.Show("Cannot delete this position as it is currently in use.");
                        return;
                    }

                    var result = MessageBox.Show($"Are you sure you want to delete position '{positionToDelete.PositionTitle}'?",
                        "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _context.DeletePosition(user, positionToDelete);
                        Positions.Remove(position);
                    }
                }
            }
            catch (Exception ex)
            {
                _context.LogError(user, "DeletePosition", ex);
            }
        }
    }
}