using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HumanResourcesApp.Classes;
using HumanResourcesApp.DBClasses;
using HumanResourcesApp.Models;

namespace HumanResourcesApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        // Properties for binding
        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
                ValidateUsername();
            }
        }

        private string _usernameError;
        public string UsernameError
        {
            get => _usernameError;
            set
            {
                _usernameError = value;
                OnPropertyChanged();
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private string _passwordError;
        public string PasswordError
        {
            get => _passwordError;
            set
            {
                _passwordError = value;
                OnPropertyChanged();
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
                HasError = !string.IsNullOrEmpty(value);
            }
        }

        private bool _hasError;
        public bool HasError
        {
            get => _hasError;
            set
            {
                _hasError = value;
                OnPropertyChanged();
            }
        }

        // Command for login button
        public ICommand LoginCommand { get; }

        // Constructor
        public LoginViewModel()
        {
            HumanResourcesDB humanResourcesDB = new HumanResourcesDB();
            humanResourcesDB.GetRoleById(11);
            _errorMessage = string.Empty;
            _username = string.Empty;
            _password = string.Empty;
            _usernameError = string.Empty;
            _passwordError = string.Empty;
            LoginCommand = new LoginCommandImpl(this);
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
        private void ExecuteLogin(object parameter)
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

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private class LoginCommandImpl : ICommand
        {
            private readonly LoginViewModel _viewModel;

            public LoginCommandImpl(LoginViewModel viewModel)
            {
                _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            }

            public bool CanExecute(object? parameter) => true;

            public void Execute(object? parameter)
            {
                if (parameter == null)
                {
                    throw new ArgumentNullException(nameof(parameter));
                }
                _viewModel.ExecuteLogin(parameter);
            }

            public event EventHandler? CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value!;
                remove => CommandManager.RequerySuggested -= value!;
            }
        }
    }
}