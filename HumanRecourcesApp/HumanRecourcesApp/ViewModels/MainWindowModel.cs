using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using HumanResourcesApp.DBClasses;
using HumanResourcesApp.Models;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumanResourcesApp.Views;
using HumanResourcesApp;
using System.Windows;

namespace HumanRecourcesApp.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private string _userFullName = string.Empty;
        private string _userRoleName = string.Empty;
        private Role _userRole;

        public string UserFullName
        {
            get => _userFullName;
            set
            {
                if (_userFullName != value)
                {
                    _userFullName = value;
                    OnPropertyChanged(nameof(UserFullName));
                }
            }
        }

        public string UserRoleName
        {
            get => _userRoleName;
            set
            {
                if (_userRoleName != value)
                {
                    _userRoleName = value;
                    OnPropertyChanged(nameof(UserRoleName));
                }
            }
        }

        private Page _currentPage;
        public Page CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        private string _currentPageTitle;
        public string CurrentPageTitle
        {
            get => _currentPageTitle;
            set => SetProperty(ref _currentPageTitle, value);
        }

        public ICommand NavigateCommand { get; }

        public ICommand LogoutCommand { get; }

        public MainWindowViewModel(User user)
        {
            if (user != null)
            {
                NavigateCommand = new RelayCommand<object>(ExecuteNavigate);
                LogoutCommand = new RelayCommand(ExecuteLogOut);
                UserFullName = $"{user.FirstName} {user.LastName}";
                _userRole = user.Role;
                UserRoleName = user.Role?.RoleName ?? "Unknown";
                CurrentPage = new DashboardPage(UserFullName);
                CurrentPageTitle = "Dashboard";
            }
        }

        private void ExecuteNavigate(object? parameter)
        {
            string pageName = parameter as string;

            switch (pageName)
            {
                case "Dashboard":
                    CurrentPage = new DashboardPage(UserFullName);
                    CurrentPageTitle = "Dashboard";
                    break;
                default:
                    CurrentPage = null;
                    break;
            }
        }

        private void ExecuteLogOut()
        {
            var loginWindow = new LoginView();
            loginWindow.Show();

            // Close the login window
            Application.Current.Windows[0].Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
