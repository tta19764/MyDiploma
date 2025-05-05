using CommunityToolkit.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HumanResourcesApp.Models;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using HumanResourcesApp.Views;
using HumanRecourcesApp.ViewModels;
using HumanResourcesApp.DBClasses;

namespace HumanResourcesApp.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        // Properties for summary cards
        private readonly User user;
        private readonly HumanResourcesDB _context;
        public int TotalEmployeesCount { get; set; }
        public double EmployeeChangePercentage { get; set; }
        public double AttendancePercentage { get; set; }
        public int AbsentEmployeesCount { get; set; }
        public int PendingTimeOffRequestsCount { get; set; }
        public int UpcomingReviewsCount { get; set; }
        public string CurrentDate => DateTime.Now.ToString("ddd, d MMMM yyyy");
        private readonly MainWindowViewModel _window;

        // Commands for quick action

        public DashboardViewModel(MainWindowViewModel mainWindowViewModel, User user)
        {
            _context = new HumanResourcesDB();
            _window = mainWindowViewModel;

            this.user = user;
            // Load dashboard data
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            // In a real app, this would load from your services/repositories
            TotalEmployeesCount = CountTotalEmployees();
            EmployeeChangePercentage = 3.2;
            AttendancePercentage = CountPresentEmployeesPercentage();
            AbsentEmployeesCount = CountTotalEmployees() - CountPresentEmployees();
            PendingTimeOffRequestsCount = CountPendingTimeOffRequests();
            UpcomingReviewsCount = CountUpcomingReviews();
        }

        private double CountPresentEmployeesPercentage()
        {
            return (double)CountPresentEmployees() / (double)CountTotalEmployees() * 100;
        }

        private int CountPresentEmployees()
        {
            if (user.Role.RoleName == "Admin")
            {
                int presentEmployees = _context.GetAllAttendances()
                    .Where(a => a.CheckInTime.HasValue && (a.CheckInTime.Value.Date == DateTime.Now.Date || !a.CheckOutTime.HasValue || a.CheckOutTime.Value.Date == DateTime.Now.Date)).Count();

                return presentEmployees;
            }
            else if (user == null || user.Employee == null)
            {
                return 0;
            }
            else
            {
                int presentEmployees = _context.GetAllAttendances()
                    .Where(a => a.Employee.Department.Manager != null && a.Employee.Department.ManagerId == user.Employee.EmployeeId && a.CheckInTime.HasValue && (a.CheckInTime.Value.Date == DateTime.Now.Date || !a.CheckOutTime.HasValue || a.CheckOutTime.Value.Date == DateTime.Now.Date)).Count();

                return presentEmployees;
            }
        }

        private int CountTotalEmployees()
        {
            if (user.Role.RoleName == "Admin")
            {
                return _context.GetAllEmplyees().Count();
            }
            else if (user == null || user.Employee == null)
            {
                return 0;
            }
            else
            {
                return _context.GetAllEmplyees()
                    .Where(e => e.Department.Manager != null && e.Department.ManagerId == user.Employee.EmployeeId).Count();
            }
        }

        private int CountPendingTimeOffRequests()
        {
            if (user.Role.RoleName == "Admin")
            {
                return _context.GetAllTimeOffRequests()
                    .Where(r => r.Status == "Pending").Count();
            }
            else if (user == null || user.Employee == null)
            {
                return 0;
            }
            else
            {
                return _context.GetAllTimeOffRequests()
                .Where(r => r.Employee.Department.Manager != null && r.Employee.Department.ManagerId == user.Employee.EmployeeId && r.Status == "Pending").Count();
            }
        }

        private int CountUpcomingReviews()
        {
            if(user.Role.RoleName == "Admin")
            {
                return _context.GetAllPerformanceReviews()
                    .Where(r => r.Status == "Pending" && r.ReviewDate <= DateOnly.FromDateTime(DateTime.Now.AddDays(30))).Count();
            }
            else if (user == null || user.Employee == null)
            {
                return 0;
            }
            else
            {
                return _context.GetAllPerformanceReviews()
                .Where(r => r.ReviewerId == user.Employee.EmployeeId && r.Status == "Pending" && r.ReviewDate <= DateOnly.FromDateTime(DateTime.Now.AddDays(30))).Count();
            }
        }

        // Command execution methods
        [RelayCommand]
        private void AddEmployee()
        {
            // Implementation for navigating to add employee page
            var formWindow = new Views.EmployeeFormWindow();
            var viewModel = new EmployeeFormViewModel();
            formWindow.DataContext = viewModel;

            viewModel.RequestClose += (sender, result) =>
            {
                formWindow.DialogResult = result;
                formWindow.Close();
            };

            formWindow.ShowDialog();
        }
        [RelayCommand]
        private void ApproveTimeOff()
        {
            // Implementation for navigating to time off approvals
            _window.CurrentPage = new TimeOffRequestsPage();

        }
        [RelayCommand]
        private void ScheduleReview()
        {
            // Implementation for navigating to schedule review
            var formWindow = new PerformanceReviewFormWindow();
            formWindow.ShowDialog();
        }
        [RelayCommand]
        private void ProcessPayroll()
        {
            // Implementation for navigating to process payroll
            _window.CurrentPage = new ProcessPayrollPage(_window, user);
        }
    }

    



}