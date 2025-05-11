using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using HumanResourcesApp.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using HumanResourcesApp.DBClasses;
using CommunityToolkit.Mvvm.ComponentModel;
using HumanRecourcesApp.ViewModels;
using HumanResourcesApp.Views;

namespace HumanRecourcesApp.ViewModels
{
    public partial class DepartmentViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;
        [ObservableProperty] private ObservableCollection<DepartmentDisplayModel> departments;
        private Department? selectedDepartment; // Changed to nullable  
        [ObservableProperty] private string statusMessage = string.Empty;
        [ObservableProperty] private int totalCount;
        private readonly User user;
        [ObservableProperty] private bool canManageDepartments;

        public Department? SelectedDepartment // Changed to nullable  
        {
            get => selectedDepartment;
            set
            {
                if (SetProperty(ref selectedDepartment, value) && value != null)
                {
                    StatusMessage = $"Selected: {value.DepartmentName}";
                }
            }
        }

        public DepartmentViewModel(User _user)
        {
            _context = new HumanResourcesDB();
            Departments = new ObservableCollection<DepartmentDisplayModel>();
            user = _user;

            CanManageDepartments = _context.HasPermission(user, "SystemSettings");

            // Load data  
            LoadDepartments();
        }
        [RelayCommand]
        public void Refresh()
        {
            LoadDepartments();
        }

        public void LoadDepartments()
        {
            try
            {
                StatusMessage = "Loading departments...";

                // Load departments with related manager data  
                var departments = _context.GetAllDepartments();

                Departments = new ObservableCollection<DepartmentDisplayModel>();
                foreach (var department in departments)
                {
                    var manager = _context.GetAllEmplyees()
                        .FirstOrDefault(e => e.EmployeeId == department.ManagerId);
                    Departments.Add(new DepartmentDisplayModel
                    {
                        DepartmentId = department.DepartmentId,
                        DepartmentName = department.DepartmentName,
                        Description = department.Description,
                        ManagerId = department.ManagerId,
                        CreatedAt = department.CreatedAt,
                        Employees = department.Employees,
                        ManagerFullName = manager != null ? $"{manager.FirstName} {manager.LastName}" : "No Manager"
                    });
                }
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

        [RelayCommand]
        private void AddDepartment()
        {
            var window = new DepartmentFormWindow(user);

            if (window.ShowDialog() == true)
            {
                LoadDepartments();
                StatusMessage = "New department added successfully";
            }
        }

        [RelayCommand]
        private void EditDepartment(DepartmentDisplayModel department)
        {
            if (department != null)
            {
                var departmentEntity = _context.GetDepartmentById(department.DepartmentId);
                if (departmentEntity != null)
                {
                    var window = new DepartmentFormWindow(user, departmentEntity);

                    if (window.ShowDialog() == true)
                    {
                        LoadDepartments();
                        StatusMessage = $"Department '{department.DepartmentName}' updated successfully";
                    }
                }
                else
                {
                    MessageBox.Show("The selected department could not be found in the database.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private void DeleteDepartment(DepartmentDisplayModel department)
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
                    var departmentEntity = _context.GetDepartmentById(department.DepartmentId);

                    if (departmentEntity != null)
                    {
                        _context.DeleteDepartment(user, departmentEntity);

                        // Refresh the grid  
                        LoadDepartments();
                        StatusMessage = $"Department '{departmentName}' deleted successfully";
                    }
                    else
                    {
                        StatusMessage = "Error: Department not found in the database.";
                        MessageBox.Show("The selected department could not be found in the database.",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    _context.LogError(user, "DeleteDepartment", ex);
                }
            }
        }
    }
}