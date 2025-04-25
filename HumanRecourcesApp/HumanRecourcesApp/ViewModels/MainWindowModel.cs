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
                UserRole = user.Role;
                UserRoleName = user.Role?.RoleName ?? "Unknown";
                CurrentPage = new DashboardPage(UserFullName);
                if(user.Employee != null)
                {
                    UserFullName = $"{user.Employee.FirstName} {user.Employee.LastName}";
                    UserInitials = $"{user.Employee.FirstName[0]}{user.Employee.LastName[0]}".ToUpper();
                }
                else
                {
                    UserFullName = "Unknown User";
                    UserInitials = "??";
                }
            }
            else
            {
                UserFullName = "Guest";
                UserInitials = "??";
                UserRoleName = "Guest";
                UserRole = new Role
                {
                    RoleName = "Guest"
                };
                CurrentPage = new DashboardPage(UserFullName);
            }
        }

        [RelayCommand]
        private void Navigate(object? parameter)
        {
            string? pageName = parameter as string;

            if (pageName == null)
            {
                CurrentPage = new DashboardPage(UserFullName); // Replace null with a default Page instance  
                return;
            }

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
                case "TimeOffRequests":
                    CurrentPage = new TimeOffRequestsPage();
                    break;
                case "Roles":
                    CurrentPage = new RolesPage();
                    break;
                case "Users":
                    CurrentPage = new UsersPage();
                    break;
                default:
                    CurrentPage = new DashboardPage(UserFullName); // Replace null with a default Page instance  
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
