using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using HumanResourcesApp.Controls;

namespace HumanRecourcesApp.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject, IPermissionContext
    {
        private readonly HumanResourcesDB _context;
        private readonly User _user;
        [ObservableProperty] private string userFullName = string.Empty;
        [ObservableProperty] private string userRoleName = string.Empty;
        [ObservableProperty] private string userInitials = string.Empty;
        [ObservableProperty] private Role userRole;
        [ObservableProperty] private Page currentPage;

        // Section permissions lists
        [ObservableProperty] private List<string> employeeSectionPermissions = new List<string>();
        [ObservableProperty] private List<string> timeSectionPermissions = new List<string>();
        [ObservableProperty] private List<string> performanceSectionPermissions = new List<string>();
        [ObservableProperty] private List<string> payrollSectionPermissions = new List<string>();
        [ObservableProperty] private List<string> systemSectionPermissions = new List<string>();

        // Permission context property - self-reference since we implement IPermissionContext
        public IPermissionContext PermissionContext => this;

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

            // Initialize permission lists for each section
            InitializePermissionLists();
        }

        // Initialize permission lists for each section
        private void InitializePermissionLists()
        {
            // Employee Management section permissions
            EmployeeSectionPermissions = new List<string>
            {
                "ViewEmployees",
                "",
                "EditEmployees"
            };

            // Time Management section permissions
            TimeSectionPermissions = new List<string>
            {
                "ViewAttendance",
                "ViewAttendance",
                "ViewAttendance"
            };

            // Performance section permissions
            PerformanceSectionPermissions = new List<string>
            {
                "",
                ""
            };

            // Payroll section permissions
            PayrollSectionPermissions = new List<string>
            {
                "ViewPayroll",
                "ViewPayroll",
                "ViewPayroll"
            };

            // System section permissions
            SystemSectionPermissions = new List<string>
            {
                "ManageUsers",
                "ManageRoles",
                "SystemSettings"
            };
        }

        // Implement IPermissionContext interface
        public bool UserHasAccess(string permission)
        {
            // If permission is empty, access is granted
            if (string.IsNullOrEmpty(permission))
            {
                return true;
            }

            // Call the existing UserHasAccess method from your context
            return _context.HasPermission(_user, permission);
        }

        [RelayCommand]
        private void Navigate(object? parameter)
        {
            string? pageName = parameter as string;

            if (pageName == null)
            {
                CurrentPage = new DashboardPage(this, _user);
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
                case "ViewPerformance":
                    CurrentPage = new PerformanceReviewsPage(_user);
                    break;
                case "PayrollItems":
                    CurrentPage = new PayrollItemsPage(_user);
                    break;
                case "ProcessPayroll":
                    CurrentPage = new ProcessPayrollPage(this, _user);
                    break;
                default:
                    CurrentPage = new DashboardPage(this, _user);
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