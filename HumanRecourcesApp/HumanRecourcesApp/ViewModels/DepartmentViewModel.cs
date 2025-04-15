using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using HumanResourcesApp.Models;
//using HumanResourcesApp.Windows;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using HumanResourcesApp.DBClasses;
using CommunityToolkit.Mvvm.ComponentModel;
using HumanRecourcesApp.ViewModels;
using HumanResourcesApp.Views;

namespace HumanRecourcesApp.ViewModels
{
    public class DepartmentViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;
        private ObservableCollection<Department> _departments;
        private Department _selectedDepartment;
        private string _statusMessage;
        private int _totalCount;

        public ObservableCollection<Department> Departments
        {
            get => _departments;
            set => SetProperty(ref _departments, value);
        }

        public Department SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                if (SetProperty(ref _selectedDepartment, value) && value != null)
                {
                    StatusMessage = $"Selected: {value.DepartmentName}";
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public int TotalCount
        {
            get => _totalCount;
            set => SetProperty(ref _totalCount, value);
        }

        // Commands
        public ICommand AddDepartmentCommand { get; }
        public ICommand EditDepartmentCommand { get; }
        public ICommand DeleteDepartmentCommand { get; }
        public ICommand RefreshCommand { get; }

        public DepartmentViewModel()
        {
            _context = new HumanResourcesDB();
            Departments = new ObservableCollection<Department>();

            // Initialize commands
            AddDepartmentCommand = new RelayCommand(AddDepartment);
            EditDepartmentCommand = new RelayCommand<Department>(EditDepartment);
            DeleteDepartmentCommand = new RelayCommand<Department>(DeleteDepartment);
            RefreshCommand = new RelayCommand(LoadDepartmentsAsync);

            // Load data
            LoadDepartmentsAsync();
        }

        public async void LoadDepartmentsAsync()
        {
            try
            {
                StatusMessage = "Loading departments...";

                // Load departments with related manager data
                var departments = await _context.GetAllDepartmentsAsync();

                Departments = new ObservableCollection<Department>(departments);
                TotalCount = Departments.Count;
                StatusMessage = "Departments loaded successfully";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading departments: {ex.Message}";
                MessageBox.Show($"Error loading departments: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddDepartment()
        {
            var window = new DepartmentFormWindow();

            if (window.ShowDialog() == true)
            {
                LoadDepartmentsAsync();
                StatusMessage = "New department added successfully";
            }
        }

        private void EditDepartment(Department department)
        {
            if (department != null)
            {
                var window = new DepartmentFormWindow(department);

                if (window.ShowDialog() == true)
                {
                    LoadDepartmentsAsync();
                    StatusMessage = $"Department '{department.DepartmentName}' updated successfully";
                }
            }
        }

        private async void DeleteDepartment(Department department)
        {
            if (department == null) return;

            // Check if the department has employees
            if (department.Employees.Count > 0)
            {
                MessageBox.Show(
                    $"Cannot delete department '{department.DepartmentName}' because it has {department.Employees.Count} employee(s) assigned to it. " +
                    "Reassign employees to other departments before deleting.",
                    "Cannot Delete Department",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // Confirm deletion
            var result = MessageBox.Show(
                $"Are you sure you want to delete the department '{department.DepartmentName}'?",
                "Confirm Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    string departmentName = department.DepartmentName;
                    _context.DeleteDepartment(department);

                    // Refresh the grid
                    LoadDepartmentsAsync();
                    StatusMessage = $"Department '{departmentName}' deleted successfully";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Error deleting department: {ex.Message}";
                    MessageBox.Show($"Error deleting department: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}