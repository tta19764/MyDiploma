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
    public class DepartmentFormViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;
        private readonly bool _isEditMode;

        private string _formTitle;
        private string _departmentName;
        private string _description;
        private DateTime? _createdAt;
        private Employee _selectedManager;
        private ObservableCollection<Position> _positions;
        private Position _selectedPosition;
        private string _validationMessage;
        private ObservableCollection<Employee> _managers;

        // Properties
        public string FormTitle
        {
            get => _formTitle;
            set => SetProperty(ref _formTitle, value);
        }

        public string DepartmentName
        {
            get => _departmentName;
            set => SetProperty(ref _departmentName, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

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

        public Employee SelectedManager
        {
            get => _selectedManager;
            set => SetProperty(ref _selectedManager, value);
        }

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
                        Managers = new ObservableCollection<Employee>(_context.GetAllFreeEmplyeesByPosition(value));
                    }
                    else
                    {
                        Managers = new ObservableCollection<Employee>();
                    }
                }
            }
        }

        public string ValidationMessage
        {
            get => _validationMessage;
            set => SetProperty(ref _validationMessage, value);
        }

        public ObservableCollection<Employee> Managers
        {
            get => _managers;
            set => SetProperty(ref _managers, value);
        }

        public ObservableCollection<Position> Positions
        {
            get => _positions;
            set => SetProperty(ref _positions, value);
        }

        // The department being added or edited
        public Department Department { get; }

        // Commands
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        // Constructor for Add mode
        public DepartmentFormViewModel()
        {
            _context = new HumanResourcesDB();
            Department = new Department
            {
                CreatedAt = DateTime.Now
            };
            _isEditMode = false;

            FormTitle = "Add New Department";
            CreatedAt = DateTime.Now;

            SaveCommand = new RelayCommand<Window>(Save);
            CancelCommand = new RelayCommand<Window>(Cancel);

            // Load managers
            LoadManagers();
        }

        // Constructor for Edit mode
        public DepartmentFormViewModel(Department department)
        {
            _context = new HumanResourcesDB();
            Department = department;
            _isEditMode = true;

            FormTitle = $"Edit Department: {department.DepartmentName}";

            // Initialize properties from department
            DepartmentName = department.DepartmentName;
            Description = department.Description;
            CreatedAt = department.CreatedAt;

            SaveCommand = new RelayCommand<Window>(Save);
            CancelCommand = new RelayCommand<Window>(Cancel);

            // Load managers
            LoadManagers();

        }

        private void LoadManagers()
        {
            try
            {
                Positions = new ObservableCollection<Position>(_context.GetAllPositions());
                

                // Set selected manager if in edit mode
                if (_isEditMode && Department.ManagerId.HasValue)
                {
                    SelectedPosition = _context.GetPositionById(Department.Manager.PositionId);
                    Managers = new ObservableCollection<Employee>(_context.GetAllFreeEmplyeesByPosition(SelectedPosition));
                    SelectedManager = _context.GetAllEmplyeesByPosition(SelectedPosition).FirstOrDefault(m => m.EmployeeId == Department.ManagerId);
                }
            }
            catch (Exception ex)
            {
                ValidationMessage = $"Error loading managers: {ex.Message}";
            }
        }

        private void Save(object parameter)
        {
            if (ValidateForm())
            {
                try
                {

                    if (_isEditMode)
                    {
                        _context.UpdateDepartment(Department);
                    }
                    else
                    {
                        _context.AddDepartment(Department);
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
                    ValidationMessage = $"Error saving department: {ex.Message}";
                }
            }
        }

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

            Department.DepartmentName = DepartmentName?.Trim();
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