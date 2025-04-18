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
        private string _userInitials = string.Empty;
        private Role _userRole;

        public string UserFullName
        {
            get => _userFullName;
            set => SetProperty(ref _userFullName, value);
        }

        public string UserRoleName
        {
            get => _userRoleName;
            set => SetProperty(ref _userRoleName, value);
        }

        private Page _currentPage;
        public Page CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        public string UserInitials
        {
            get => _userInitials;
            set => SetProperty(ref _userInitials, value);
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
                UserInitials = $"{user.FirstName[0]}{user.LastName[0]}".ToUpper();
            }
        }

        private void ExecuteNavigate(object? parameter)
        {
            string pageName = parameter as string;

            switch (pageName)
            {
                case "Dashboard":
                    CurrentPage = new DashboardPage(UserFullName);
                    break;
                case "Departments":
                    CurrentPage = new DepartmentPage();
                    break;
                case "Positions":
                    CurrentPage = new PositionPage();
                    break;
                case "Employees":
                    CurrentPage = new EmployeePage();
                    break;
                case "Attendance":
                    CurrentPage = new AttendancePage();
                    break;
                case "TimeOffTypes":
                    CurrentPage = new TimeOffTypesPage();
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
    }
}
