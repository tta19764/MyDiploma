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
        private readonly Employee _employee;

        [ObservableProperty]
        private ObservableCollection<Employee> employees;

        [ObservableProperty]
        private Employee selectedEmployee;

        public EmployeeViewModel()
        {
            _context = new HumanResourcesDB();
            _employee = new Employee();
            LoadEmployees();
        }

        private async void LoadEmployees()
        {
            try
            {
                var employeeList = _context.GetAllEmployees();
                Employees = new ObservableCollection<Employee>(employeeList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employees: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void AddEmployee()
        {
            var formWindow = new Views.EmployeeFormWindow();
            var viewModel = new EmployeeFormViewModel();
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
            if (employee == null) return;

            var formWindow = new Views.EmployeeFormWindow();
            var viewModel = new EmployeeFormViewModel(employee);
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

        [RelayCommand]
        private async Task DeleteEmployee(Employee employee)
        {
            if (employee == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete {employee.FirstName} {employee.LastName}?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _context.DeleteEmployee(employee);
                    Employees.Remove(employee);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting employee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}