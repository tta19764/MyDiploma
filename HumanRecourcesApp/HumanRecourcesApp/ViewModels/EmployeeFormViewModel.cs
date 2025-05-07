using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumanResourcesApp.DBClasses;
using HumanResourcesApp.Models;

namespace HumanResourcesApp.ViewModels
{
    public partial class EmployeeFormViewModel : ObservableObject
    {
        private readonly bool _isNewEmployee;
        private readonly HumanResourcesDB _context;
        private readonly User user;
        [ObservableProperty] private Employee employee;
        [ObservableProperty] private string userIdString;

        private string salaryString;
        public string SalaryString
        {
            get => salaryString;
            set
            {
                if (decimal.TryParse(value, out decimal salary))
                {
                    Employee.Salary = salary;
                    SetProperty(ref salaryString, value);
                }
                else if (string.IsNullOrWhiteSpace(value))
                {
                    Employee.Salary = 0;
                    SetProperty(ref salaryString, value);
                }
                else
                {
                    MessageBox.Show("Invalid salary format.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        [ObservableProperty]
        private string windowTitle;

        [ObservableProperty]
        private ObservableCollection<Department> departments;

        [ObservableProperty]
        private ObservableCollection<Position> positions;

        // Event used to communicate with the view
        public event EventHandler<bool> RequestClose;

        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        private static readonly Regex PhoneRegex = new(@"^\+?[0-9\s\-]{7,15}$", RegexOptions.Compiled);


        public EmployeeFormViewModel(User _user)
        {
            _context = new HumanResourcesDB();
            user = _user;
            _isNewEmployee = true;
            windowTitle = "Add New Employee";
            Employee = new Employee
            {
                IsActive = true,
                HireDate = DateOnly.FromDateTime(DateTime.Today)
            };
            UserIdString = "Account is not linked";
            SalaryString = string.Empty;

            LoadDepartmentsAndPositions();
        }

        public EmployeeFormViewModel(User _user, Employee employeeToEdit)
        {
            _context = new HumanResourcesDB();
            user = _user;
            // Create a copy of the employee to avoid modifying the original directly
            Employee = new Employee
            {
                EmployeeId = employeeToEdit.EmployeeId,
                UserId = employeeToEdit.UserId,
                FirstName = employeeToEdit.FirstName,
                LastName = employeeToEdit.LastName,
                MiddleName = employeeToEdit.MiddleName,
                DateOfBirth = employeeToEdit.DateOfBirth,
                Gender = employeeToEdit.Gender,
                Address = employeeToEdit.Address,
                City = employeeToEdit.City,
                Country = employeeToEdit.Country,
                Phone = employeeToEdit.Phone,
                Email = employeeToEdit.Email,
                HireDate = employeeToEdit.HireDate,
                TerminationDate = employeeToEdit.TerminationDate,
                PositionId = employeeToEdit.PositionId,
                Salary = employeeToEdit.Salary,
                IsActive = employeeToEdit.IsActive,
                DepartmentId = employeeToEdit.DepartmentId
            };

            UserIdString = (Employee.UserId == null) ? "Account is not linked" : Employee.UserId.ToString() ?? "Account is not linked";
            SalaryString = Employee.Salary.ToString();

            _isNewEmployee = false;
            windowTitle = "Edit Employee";

            LoadDepartmentsAndPositions();
        }

        private void LoadDepartmentsAndPositions()
        {
            try
            {
                var deptList = _context.GetAllDepartments();
                Departments = new ObservableCollection<Department>(deptList);

                var posList = _context.GetAllPositions();
                Positions = new ObservableCollection<Position>(posList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void SaveEmployee()
        {
            if (!ValidateEmployee())
            {
                return;
            }

            try
            {
                if (_isNewEmployee)
                {
                    _context.AddEmployee(user, Employee);
                }
                else
                {
                    _context.UpdateEmployee(Employee);
                }

                CloseWindow(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving employee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseWindow(false);
        }

        private bool ValidateEmployee()
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(Employee.FirstName))
            {
                MessageBox.Show("First name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(Employee.LastName))
            {
                MessageBox.Show("Last name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(Employee.Email) && !EmailRegex.IsMatch(Employee.Email))
            {
                MessageBox.Show("Email format is invalid.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(Employee.Phone) && !PhoneRegex.IsMatch(Employee.Phone))
            {
                MessageBox.Show("Phone number format is invalid.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (Employee.DepartmentId <= 0)
            {
                MessageBox.Show("Please select a department.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (Employee.PositionId <= 0)
            {
                MessageBox.Show("Please select a position.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void CloseWindow(bool result)
        {
            RequestClose?.Invoke(this, result);
        }
    }
}