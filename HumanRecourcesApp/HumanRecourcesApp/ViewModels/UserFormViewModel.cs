using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumanResourcesApp.Models;
using HumanResourcesApp.DBClasses;
using HumanResourcesApp.Views;
using System.Data;
using System.Windows.Controls;

namespace HumanResourcesApp.ViewModels
{
    public partial class UserFormViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;
        private readonly Window _window;
        private readonly Random _random = new Random();

        private bool _isPasswordVisible = false;
        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set
            {
                _isPasswordVisible = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PasswordVisibilityTooltip));
                OnPropertyChanged(nameof(PasswordVisibilityIcon));
            }
        }

        public string PasswordVisibilityTooltip => IsPasswordVisible ? "Hide Password" : "Show Password";

        public string PasswordVisibilityIcon => IsPasswordVisible
    ? "E:\\C#_projects\\HumanResourcesApp\\HumanRecourcesApp\\HumanRecourcesApp\\Resources\\hidden.ico"
    : "E:\\C#_projects\\HumanResourcesApp\\HumanRecourcesApp\\HumanRecourcesApp\\Resources\\show.ico";



        [RelayCommand]
        private void TogglePasswordVisibility() 
        { 
            IsPasswordVisible = !IsPasswordVisible;

            if(_window is UserForm form)
            {
                if(!IsPasswordVisible)
                {
                    form.PasswordBox.Password = Password;
                }
                else
                {
                    Password = form.PasswordBox.Password;
                }
            }
        }

        [ObservableProperty]
        private User user;

        [ObservableProperty]
        private ObservableCollection<Role> roles = new();

        [ObservableProperty]
        private ObservableCollection<Employee> employees = new();

        [ObservableProperty]
        private Role? selectedRole;

        [ObservableProperty]
        private Employee? selectedEmployee;

        [ObservableProperty]
        private string windowTitle;

        [ObservableProperty]
        private bool isEditMode;

        [ObservableProperty]
        private string password = string.Empty;

        public UserFormViewModel(Window window)
        {
            _context = new HumanResourcesDB();
            IsEditMode = false;
            WindowTitle = "Add New User";

            // Create a new user or clone the existing one
            User = new User
            {
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            // Load reference data
            LoadRoles();
            LoadEmployees();

            // Set selected items based on current values
            SelectedRole = Roles.FirstOrDefault();
            _window = window;
        }

        public UserFormViewModel(Window window, User user)
        {
            _context = new HumanResourcesDB();
            // Determine if we're editing or creating
            IsEditMode = true;
            WindowTitle = "Edit User";

            // Create a new user or clone the existing one
            User = _context.GetUserById(user.UserId);

            // Load reference data
            LoadRoles();
            LoadEmployees();

            // Set selected items based on current values
            SelectedRole = Roles.FirstOrDefault(r => r.RoleId == User.RoleId);
            SelectedEmployee = User.Employee != null
                    ? Employees.FirstOrDefault(e => e.EmployeeId == User.Employee.EmployeeId)
                    : null;
            _window = window;
        }

        private void LoadRoles()
        {
            var roleList = _context.GetAllRoles();
            Roles = new ObservableCollection<Role>(roleList);
        }

        private void LoadEmployees()
        {
            // Get employees that don't already have a user account
            var employeesWithoutUser = _context.GetAllEmplyees()
                .Where(e => e.UserId == null || e.UserId == User.UserId)
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .ToList();

            Employees = new ObservableCollection<Employee>(employeesWithoutUser);
        }

        [RelayCommand]
        private void GeneratePassword()
        {
            // Generate a random password
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            const string specialChars = "!@#$%^&*()-_=+[]{}|;:,.<>?";

            string allChars = lowerChars + upperChars + numbers + specialChars;
            char[] password = new char[12]; // 12-character password

            // Ensure at least one of each type
            password[0] = lowerChars[_random.Next(lowerChars.Length)];
            password[1] = upperChars[_random.Next(upperChars.Length)];
            password[2] = numbers[_random.Next(numbers.Length)];
            password[3] = specialChars[_random.Next(specialChars.Length)];

            // Fill the rest randomly
            for (int i = 4; i < password.Length; i++)
            {
                password[i] = allChars[_random.Next(allChars.Length)];
            }

            // Shuffle the password characters
            for (int i = 0; i < password.Length; i++)
            {
                int randomIndex = _random.Next(password.Length);
                char temp = password[i];
                password[i] = password[randomIndex];
                password[randomIndex] = temp;
            }

            Password = new string(password);

            // Update the password box - requires code-behind
            if (_window is UserForm form)
            {
                form.PasswordBox.Password = Password;
            }
        }

        [RelayCommand]
        private void ClearEmployee()
        {
            SelectedEmployee = null;
        }

        [RelayCommand]
        private void Save()
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(User.Username))
                {
                    MessageBox.Show("Username is required", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (SelectedRole == null)
                {
                    MessageBox.Show("Role is required", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Check if username is unique
                if (!_context.IsUniqueLogin(User))
                {
                    MessageBox.Show("Username already exists", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Update user's role
                User.RoleId = SelectedRole.RoleId;

                // Update employee link
                if (SelectedEmployee != null)
                {
                    User.Employee = _context.GetEmployeeById(SelectedEmployee.EmployeeId);
                }
                else if (User.Employee != null)
                {
                    User.Employee = null;
                }

                // Handle password
                if (_window is UserForm form && !string.IsNullOrEmpty(form.PasswordBox.Password))
                {
                    // Hash the password (in real app, use a proper hashing algorithm)
                    User.PasswordHash = Classes.Login.HashPassword(form.PasswordBox.Password);
                }
                else if (!IsEditMode)
                {
                    // New user must have a password
                    MessageBox.Show("Password is required for new users", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Save changes
                if (!IsEditMode)
                {
                    _context.AddUser(User);
                }
                else
                {
                    _context.UpdateUser(User);
                }

                // Close the window
                CloseDialogWithResult(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseDialogWithResult(false);
        }

        private void CloseDialogWithResult(bool? result)
        {
            _window.DialogResult = result;
            _window.Close();
        }
    }
}