using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumanResourcesApp.DBClasses;
using HumanResourcesApp.Models;
using Microsoft.EntityFrameworkCore;

namespace HumanRecourcesApp.ViewModels
{
    public partial class DepartmentFormViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;
        private readonly User user;
        [ObservableProperty] private bool isEditMode;
        [ObservableProperty] private string formTitle = string.Empty;
        [ObservableProperty] private string departmentName = string.Empty;
        [ObservableProperty] private string description = string.Empty;
        private DateTime? _createdAt;
        [ObservableProperty] private EmployeeDisplayModel selectedManager = new EmployeeDisplayModel();
        [ObservableProperty] private ObservableCollection<Position> positions = new ObservableCollection<Position>();
        private Position _selectedPosition = new Position();
        [ObservableProperty] private string validationMessage = string.Empty;
        [ObservableProperty] private ObservableCollection<EmployeeDisplayModel> managers = new ObservableCollection<EmployeeDisplayModel>();

        // Properties

        public DateTime? CreatedAt
        {
            get => _createdAt;
            set
            {
                if (SetProperty(ref _createdAt, value))
                {
                    OnPropertyChanged(nameof(CreatedAtDisplay));
                }
            }
        }

        public string CreatedAtDisplay => CreatedAt.HasValue ? CreatedAt.Value.ToString("dddd,d MMMM yyyy") : "Not set";

        public Position SelectedPosition
        {
            get => _selectedPosition;
            set
            {
                if (SetProperty(ref _selectedPosition, value))
                {
                    // Load managers based on selected position
                    if (value != null)
                    {
                        Managers.Clear();
                        var employees = _context.GetAllFreeEmplyeesByPosition(value);
                        foreach (var employee in employees)
                        {
                            Managers.Add(new EmployeeDisplayModel
                            {
                                EmployeeId = employee.EmployeeId,
                                FirstName = employee.FirstName,
                                LastName = employee.LastName,
                                Position = employee.Position
                            });
                        }
                        if(IsEditMode && Department.ManagerId != null)
                        {
                            var currentManager = _context.GetEmployeeById(Department.ManagerId.Value);
                            if (currentManager != null)
                            {
                                Managers.Add(new EmployeeDisplayModel
                                {
                                    EmployeeId = currentManager.EmployeeId,
                                    FirstName = currentManager.FirstName,
                                    LastName = currentManager.LastName,
                                    Position = currentManager.Position
                                });
                                SelectedManager = Managers.FirstOrDefault(m => m.EmployeeId == Department.ManagerId) ?? new EmployeeDisplayModel();
                            }
                        }
                        else
                        {
                            SelectedManager = new EmployeeDisplayModel();
                        }
                        
                    }
                    else
                    {
                        Managers.Clear();
                    }
                }
            }
        }

        // The department being added or edited
        public Department Department { get; }


        // Constructor for Add mode
        public DepartmentFormViewModel(User _user)
        {
            _context = new HumanResourcesDB();
            user = _user;
            Department = new Department
            {
                CreatedAt = DateTime.Now
            };
            SelectedManager = new EmployeeDisplayModel();
            IsEditMode = false;

            FormTitle = "Add New Department";
            CreatedAt = DateTime.Now;

            // Load managers
            LoadManagers();
        }

        // Constructor for Edit mode
        public DepartmentFormViewModel(User _user, Department department)
        {
            _context = new HumanResourcesDB();
            user = _user;
            Department = department;
            IsEditMode = true;

            FormTitle = $"Edit Department: {department.DepartmentName}";

            // Initialize properties from department
            DepartmentName = department.DepartmentName;
            Description = department.Description ?? string.Empty;
            CreatedAt = department.CreatedAt;
            Managers = new ObservableCollection<EmployeeDisplayModel>();
            var employees = _context.GetAllEmplyees();
            foreach (var employee in employees)
            {
                Managers.Add(new EmployeeDisplayModel
                {
                    EmployeeId = employee.EmployeeId,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Position = employee.Position
                });
            }

            // Load managers
            LoadManagers();

        }

        private void LoadManagers()
        {
            try
            {
                Positions = new ObservableCollection<Position>(_context.GetAllPositions());
                

                // Set selected manager if in edit mode
                if (IsEditMode && Department.Manager != null)
                {
                    int positionId = Department.Manager.PositionId;
                    SelectedPosition = Positions.FirstOrDefault(p => p.PositionId == positionId) ?? new Position();
                    SelectedManager = Managers.FirstOrDefault(m => m.EmployeeId == Department.ManagerId) ?? new EmployeeDisplayModel();
                }
            }
            catch (Exception ex)
            {
                ValidationMessage = $"Error loading managers: {ex.Message}";
            }
        }

        [RelayCommand]
        private void SaveDepartment(object parameter)
        {
            if (ValidateForm())
            {
                try
                {

                    if (IsEditMode)
                    {
                        _context.UpdateDepartment(user, Department);
                    }
                    else
                    {
                        _context.AddDepartment(user, Department);
                    }


                    // Return true to indicate success
                    if (parameter is Window window)
                    {
                        window.DialogResult = true;
                        window.Close();
                    }
                }
                catch (Exception ex)
                {
                    _context.LogError(user, "SaveDepartment", ex);
                }
            }
        }

        [RelayCommand]
        private void Cancel(object parameter)
        {
            if (parameter is Window window)
            {
                window.DialogResult = false;
                window.Close();
            }
        }

        private bool ValidateForm()
        {
            // Reset validation message
            ValidationMessage = string.Empty;

            // Validate department name
            if (string.IsNullOrWhiteSpace(DepartmentName))
            {
                ValidationMessage = "Department name is required";
                return false;
            }

            // Validate department name length
            if (DepartmentName.Trim().Length > 50)
            {
                ValidationMessage = "Department name cannot exceed 50 characters";
                return false;
            }

            // Check for duplicate department name

            Department.DepartmentName = DepartmentName.Trim();
            Department.Description = Description?.Trim();
            Department.ManagerId = SelectedManager?.EmployeeId;
            Department.CreatedAt = CreatedAt;

            if (!_context.IsUniqueDepartmentName(Department))
            {
                ValidationMessage = "A department with this name already exists";
                return false;
            }

            return true;
        }
    }
}