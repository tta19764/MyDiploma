using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumanResourcesApp.DBClasses;
using HumanResourcesApp.Models;

namespace HumanResourcesApp.ViewModels
{
    public partial class EmployeeViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;

        [ObservableProperty] private ObservableCollection<Employee> employees;
        private readonly User user;
        [ObservableProperty] private Employee selectedEmployee;
        [ObservableProperty] private bool canCreateEmployees = false;
        [ObservableProperty] private bool canEditEmployees = false;
        [ObservableProperty] private bool canDeleteEmployees = false;

        public EmployeeViewModel(User _user)
        {
            _context = new HumanResourcesDB();
            user = _user;
            CanCreateEmployees = _context.HasPermission(user, "CreateEmployees");
            canEditEmployees = _context.HasPermission(user, "EditEmployees");
            canDeleteEmployees = _context.HasPermission(user, "DeleteEmployees");
            SelectedEmployee = new Employee();
            Employees = new ObservableCollection<Employee>();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            try
            {
                Employees.Clear();
                if (user.Employee != null && _context.HasPermission(user, "ViewEmployees") && (user.Role.RoleName != "Admin" && user.Role.RoleName != "HR Manager" && user.Role.RoleName != "HR Staff"))
                {
                    var employeeList = _context.GetAllEmplyees().Where(e => e.DepartmentId == user.Employee.DepartmentId);
                    Employees = new ObservableCollection<Employee>(employeeList);
                }
                else
                {
                    var employeeList = _context.GetAllEmplyees();
                    Employees = new ObservableCollection<Employee>(employeeList);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employees: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void AddEmployee()
        {
            var formWindow = new Views.EmployeeFormWindow(user);
            var viewModel = new EmployeeFormViewModel(user);
            formWindow.DataContext = viewModel;

            viewModel.RequestClose += (sender, result) =>
            {
                formWindow.DialogResult = result;
                formWindow.Close();
            };

            bool? result = formWindow.ShowDialog();
            if (result == true)
            {
                LoadEmployees(); // Reload employees after adding
            }
        }

        [RelayCommand]
        private void EditEmployee(Employee employee)
        {
            try
            {
                if (employee == null) throw new Exception("Employee not found.");

                var formWindow = new Views.EmployeeFormWindow(user);
                var viewModel = new EmployeeFormViewModel(user, employee);
                formWindow.DataContext = viewModel;

                viewModel.RequestClose += (sender, result) =>
                {
                    formWindow.DialogResult = result;
                    formWindow.Close();
                };

                bool? result = formWindow.ShowDialog();
                if (result == true)
                {
                    LoadEmployees(); // Reload employees after editing
                }
            }
            catch (Exception ex)
            {
                _context.LogError(user, "EditEmployee", ex);
            }
        }

        [RelayCommand]
        private void DeleteEmployee(Employee employee)
        {
            
            try
            {
                if (employee == null) throw new Exception("Employee not found.");

                var result = MessageBox.Show(
                    $"Are you sure you want to delete {employee.FirstName} {employee.LastName}?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (!_context.CanDeleteEmployee(employee.EmployeeId))
                    {
                        MessageBox.Show("This employee cannot be deleted because they have associated records.\nTry deactivating the employee instead.", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    _context.DeleteEmployee(user, employee);
                    Employees.Remove(employee);
                }
            }
            catch (Exception ex)
            {
                _context.LogError(user, "DeleteEmployee", ex);
            }
        }
    }
}