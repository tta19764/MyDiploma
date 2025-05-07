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
        private readonly HumanResourcesDB _context;
        private readonly User _user;
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
            _context = new HumanResourcesDB();
            if (user != null)
            {
                UserRole = user.Role;
                UserRoleName = user.Role?.RoleName ?? "Unknown";
                _user = _context.GetUserById(user.UserId) ?? user;
                CurrentPage = new DashboardPage(this, _user);
                if (user.Employee != null)
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
                _user = new User { UserId = 0, Employee = null, Role = _context.GetAllRoles().Where(r => r.RolePermissions.Count == 0).FirstOrDefault() ?? new Role() { RoleName = "None" } };
                UserRole = _user.Role ?? new Role
                {
                    RoleName = "None"
                };
                CurrentPage = new DashboardPage(this, _user);
            }
        }

        [RelayCommand]
        private void Navigate(object? parameter)
        {
            string? pageName = parameter as string;

            if (pageName == null)
            {
                CurrentPage = new DashboardPage(this, _user); // Replace null with a default Page instance  
                return;
            }

            switch (pageName)
            {
                case "Dashboard":
                    CurrentPage = new DashboardPage(this, _user);
                    break;
                case "Departments":
                    CurrentPage = new DepartmentPage(_user);
                    break;
                case "Positions":
                    CurrentPage = new PositionPage(_user);
                    break;
                case "Employees":
                    CurrentPage = new EmployeePage(_user);
                    break;
                case "Attendance":
                    CurrentPage = new AttendancePage(_user);
                    break;
                case "TimeOffTypes":
                    CurrentPage = new TimeOffTypesPage(_user);
                    break;
                case "TimeOffRequests":
                    CurrentPage = new TimeOffRequestsPage(_user);
                    break;
                case "Roles":
                    CurrentPage = new RolesPage(_user);
                    break;
                case "Users":
                    CurrentPage = new UsersPage(_user);
                    break;
                case "PayPeriods":
                    CurrentPage = new PayPeriodsPage(_user);
                    break;
                case "SystemLogs":
                    CurrentPage = new SystemLogPage(_user);
                    break;
                case "PerformanceCriteria":
                    CurrentPage = new PerformanceCriteriaPage(_user);
                    break;
                case "PerformanceReviews":
                    CurrentPage = new PerformanceReviewsPage(_user);
                    break;
                case "PayrollItems":
                    CurrentPage = new PayrollItemsPage(_user);
                    break;
                case "ProcessPayroll":
                    CurrentPage = new ProcessPayrollPage(this, _user);
                    break;
                default:
                    CurrentPage = new DashboardPage(this, _user); // Replace null with a default Page instance  
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
