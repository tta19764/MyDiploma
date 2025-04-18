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
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string userFullName = string.Empty;
        [ObservableProperty]
        private string userRoleName = string.Empty;
        [ObservableProperty]
        private string userInitials = string.Empty;
        [ObservableProperty]
        private Role userRole;
        [ObservableProperty]
        private Page currentPage;

        public MainWindowViewModel(User user)
        {
            if (user != null)
            {
                UserFullName = $"{user.FirstName} {user.LastName}";
                UserRole = user.Role;
                UserRoleName = user.Role?.RoleName ?? "Unknown";
                CurrentPage = new DashboardPage(UserFullName);
                UserInitials = $"{user.FirstName[0]}{user.LastName[0]}".ToUpper();
            }
        }

        [RelayCommand]
        private void Navigate(object? parameter)
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

        [RelayCommand]
        private void LogOut()
        {
            var loginWindow = new LoginView();
            loginWindow.Show();

            // Close the login window
            Application.Current.Windows[0].Close();
        }
    }
}
