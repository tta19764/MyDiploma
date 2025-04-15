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
    public class PositionViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;
        private ObservableCollection<PositionDisplayModel> _positions;
        private PositionDisplayModel _selectedPosition;
        private PositionDisplayModel _newPosition;
        private bool _isAddingNew;

        public ObservableCollection<PositionDisplayModel> Positions
        {
            get => _positions;
            set => SetProperty(ref _positions, value);
        }

        public PositionDisplayModel SelectedPosition
        {
            get => _selectedPosition;
            set => SetProperty(ref _selectedPosition, value);
        }

        public PositionDisplayModel NewPosition
        {
            get => _newPosition;
            set => SetProperty(ref _newPosition, value);
        }

        public bool IsAddingNew
        {
            get => _isAddingNew;
            set => SetProperty(ref _isAddingNew, value);
        }

        public ICommand AddCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveNewCommand { get; }

        public PositionViewModel()
        {
            // Initialize commands
            _context = new HumanResourcesDB();
            AddCommand = new RelayCommand(ExecuteAdd);
            SaveCommand = new RelayCommand(ExecuteSave);
            CancelCommand = new RelayCommand(ExecuteCancel);
            SaveNewCommand = new RelayCommand(ExecuteSaveNew);
            EditCommand = new RelayCommand<PositionDisplayModel>(ExecuteEdit);
            DeleteCommand = new RelayCommand<PositionDisplayModel>(ExecuteDelete);

            // Initialize collections
            Positions = new ObservableCollection<PositionDisplayModel>();
            NewPosition = new PositionDisplayModel();

            // Load data
            LoadPositions();
        }
        private void LoadPositions()
        {
            Positions.Clear();
            var positionsList = _context.GetAllPositions();
            foreach (var position in positionsList)
            {
                Positions.Add(new PositionDisplayModel { PositionId = position.PositionId, CreatedAt = position.CreatedAt, Description = position.Description, PositionTitle = position.PositionTitle, IsEdditing = false});
            }
        }

        private void ExecuteAdd()
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

        private void ExecuteSave()
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
                        _context.UpdatePosition(positionToUpdate);
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

        private void ExecuteSaveNew()
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
                    _context.AddPosition(positionToAdd);
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

        private void ExecuteCancel()
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
                    Positions[index] = new PositionDisplayModel() { PositionId = originalPosition.PositionId, CreatedAt = originalPosition.CreatedAt, Description = originalPosition.Description, PositionTitle = originalPosition.PositionTitle};
                    SelectedPosition = Positions[index];
                }

                SelectedPosition.IsEdditing = false;
            }
        }

        private void ExecuteEdit(PositionDisplayModel position)
        {
            if (position == null) return;

            // Store the selected position and enter edit mode
            SelectedPosition = position;
            SelectedPosition.IsEdditing = true;
        }

        private void ExecuteDelete(PositionDisplayModel position)
        {
            if (position == null) return;

            var result = MessageBox.Show($"Are you sure you want to delete position '{position.PositionTitle}'?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _context.DeletePosition(new Position { PositionId = position.PositionId });
                Positions.Remove(position);
            }
        }
    }
}