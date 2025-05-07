using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumanResourcesApp.Classes;
using HumanResourcesApp.DBClasses;
using HumanResourcesApp.Models;

namespace HumanResourcesApp.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        // Properties for binding
        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set
            {
                SetProperty(ref _username, value);
                ValidateUsername();
            }
        }

        [ObservableProperty]
        private string usernameError;
        [ObservableProperty]
        private string password;
        [ObservableProperty]
        private string passwordError;

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                SetProperty(ref _errorMessage, value);
                HasError = !string.IsNullOrEmpty(value);
            }
        }

        [ObservableProperty]
        private bool hasError;

        // Constructor
        public LoginViewModel()
        {
            HumanResourcesDB humanResourcesDB = new HumanResourcesDB();
            humanResourcesDB.GetRoleById(11);
            ErrorMessage = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
            UsernameError = string.Empty;
            PasswordError = string.Empty;
        }

        

        // Validation methods
        private void ValidateUsername()
        {
            UsernameError = string.IsNullOrWhiteSpace(Username) ? "Username is required" : string.Empty;
        }

        private void ValidatePassword(string password)
        {
            PasswordError = string.IsNullOrWhiteSpace(password) ? "Password is required" : string.Empty;
        }

        // Login execution
        [RelayCommand]
        private void Login(object parameter)
        {
            // Reset errors
            ErrorMessage = string.Empty;

            // Get password from parameter (PasswordBox)
            var passwordBox = parameter as PasswordBox;
            string password = passwordBox?.Password ?? string.Empty;

            // Validate inputs
            ValidateUsername();
            ValidatePassword(password);

            // Check if any validation errors
            if (!string.IsNullOrEmpty(UsernameError) || !string.IsNullOrEmpty(PasswordError)) return;

            // Attempt authentication (mock for demo purposes)
            User? authenticatedUser = AuthenticateUser(Username, password);

            if (authenticatedUser != null)
            {
                if(authenticatedUser.IsActive == false)
                {
                    ErrorMessage = "User is inactive";
                    return;
                }
                // Authentication successful - open main window
                var mainWindow = new MainWindow(authenticatedUser);
                mainWindow.Show();

                // Close the login window
                Application.Current.Windows[0].Close();
            }
            else
            {
                // Authentication failed
                ErrorMessage = "Invalid username or password";
            }
        }

        private User? AuthenticateUser(string username, string password)
        {
            HumanResourcesDB db = new HumanResourcesDB();
            return db.GetUserByLogin(username, password);
        }
    }
}