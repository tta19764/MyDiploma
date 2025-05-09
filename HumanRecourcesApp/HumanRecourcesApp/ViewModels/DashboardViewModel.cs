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
using LiveCharts;
using LiveCharts.Wpf;
using System.Linq;

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

        [ObservableProperty] private bool isAlert;
        [ObservableProperty] private string alertContent;
        [ObservableProperty] private string alertTitle;
        public string CurrentDate => DateTime.Now.ToString("ddd, d MMMM yyyy");
        private readonly MainWindowViewModel _window;

        // Department Chart Properties
        [ObservableProperty] private SeriesCollection departmentSeries;
        [ObservableProperty] private ObservableCollection<DepartmentLegendItem> departmentLegendItems;

        // Commands for quick action

        public DashboardViewModel(MainWindowViewModel mainWindowViewModel, User user)
        {
            _context = new HumanResourcesDB();
            _window = mainWindowViewModel;
            AlertTitle = "";
            AlertContent = "";
            IsAlert = false;

            this.user = user;

            // Initialize chart collections
            DepartmentSeries = new SeriesCollection();
            DepartmentLegendItems = new ObservableCollection<DepartmentLegendItem>();

            // Load dashboard data
            LoadDashboardData();

            // Load department chart data
            LoadDepartmentChart();
        }

        private void LoadDashboardData()
        {
            // In a real app, this would load from your services/repositories
            TotalEmployeesCount = CountTotalEmployees();
            EmployeeChangePercentage = CountEmployeesMonthlyChangePercentage();
            AttendancePercentage = CountPresentEmployeesPercentage();
            AbsentEmployeesCount = CountTotalEmployees() - CountPresentEmployees();
            PendingTimeOffRequestsCount = CountPendingTimeOffRequests();
            UpcomingReviewsCount = CountUpcomingReviews();
            // Check for alerts
            CheckForAlerts();
        }

        private void LoadDepartmentChart()
        {
            // Clear previous data
            DepartmentSeries.Clear();
            DepartmentLegendItems.Clear();

            // Department colors - extend as needed
            var departmentColors = new List<Color>
            {
                Color.FromRgb(67, 97, 238),   // Blue
                Color.FromRgb(112, 72, 232),  // Purple
                Color.FromRgb(255, 107, 107), // Red
                Color.FromRgb(72, 207, 173),  // Green
                Color.FromRgb(252, 196, 83),  // Yellow
                Color.FromRgb(248, 126, 217), // Pink
                Color.FromRgb(66, 176, 255),  // Light Blue
                Color.FromRgb(152, 170, 179), // Gray
            };

            // Get all departments
            var departments = _context.GetAllDepartments().ToList();

            // Get employee distribution by department
            var employeeDistribution = new List<(int DepartmentId, string DepartmentName, int Count)>();

            employeeDistribution = _context.GetAllActiveEmployees()
                    .GroupBy(e => e.DepartmentId)
                    .Select(g => (
                        DepartmentId: g.Key,
                        DepartmentName: departments.FirstOrDefault(d => d.DepartmentId == g.Key)?.DepartmentName ?? "Unknown",
                        Count: g.Count()
                    ))
                    .ToList();

            // Calculate total for percentages
            int totalEmployees = employeeDistribution.Sum(d => d.Count);

            // Add each department to the chart
            for (int i = 0; i < employeeDistribution.Count; i++)
            {
                var dept = employeeDistribution[i];
                var colorIndex = i % departmentColors.Count;
                var color = departmentColors[colorIndex];
                var solidColorBrush = new SolidColorBrush(color);

                // Calculate percentage
                double percentage = totalEmployees > 0 ? (double)dept.Count / totalEmployees * 100 : 0;

                // Add pie segment
                DepartmentSeries.Add(new PieSeries
                {
                    Title = dept.DepartmentName,
                    Values = new ChartValues<int> { dept.Count },
                    DataLabels = false,
                    Fill = solidColorBrush,
                    Stroke = Brushes.White,
                    StrokeThickness = 2
                });

                // Add legend item
                DepartmentLegendItems.Add(new DepartmentLegendItem
                {
                    DepartmentName = dept.DepartmentName,
                    Color = solidColorBrush,
                    Count = dept.Count,
                    Percentage = percentage
                });
            }
        }

        private void CheckForAlerts()
        {
            AlertContent = "";
            AlertTitle = "";
            IsAlert = false;



            if (user.Role.RoleName == "Admin" && _context.GetAllPerformanceReviews()
                    .Where(r => r.Status == "Pending" && r.ReviewDate <= DateOnly.FromDateTime(DateTime.Now.AddDays(14))).Any())
            {
                IsAlert = true;
                AlertTitle = "Upcoming Reviews";
                AlertContent = "There are performance reviews scheduled within the next 14 days.";
            }
            else if (user == null || user.Employee == null || user.Employee.Departments.Count == 0)
            {
                return;
            }
            else if (_context.GetAllPerformanceReviews()
                    .Where(r => r.ReviewerId == user.Employee.EmployeeId && r.Status == "Pending" && r.ReviewDate <= DateOnly.FromDateTime(DateTime.Now.AddDays(14))).Any())
            {
                IsAlert = true;
                AlertTitle = "Upcoming Reviews";
                AlertContent = "There are performance reviews scheduled within the next 14 days.";
            }

            if (user.Role.RoleName == "Admin" && _context.GetAllTimeOffRequests()
                    .Where(r => r.Status == "Pending" && r.StartDate <= DateOnly.FromDateTime(DateTime.Now.AddDays(14))).Any())
            {
                if (!IsAlert)
                {
                    IsAlert = true;
                    AlertTitle = "Pending Time Off Requests";
                    AlertContent = "There are pending time off requests that need your attention.";
                }
                else
                {
                    AlertContent += "\nThere are pending time off requests that need your attention.";
                    AlertTitle += ", Pending Time Off Requests";
                }
            }
            else if (user.Employee != null && _context.GetAllTimeOffRequests()
                    .Where(r => r.Status == "Pending" && r.Employee.DepartmentId == user.Employee.DepartmentId && r.StartDate <= DateOnly.FromDateTime(DateTime.Now.AddDays(14))).Any())
            {
                if (!IsAlert)
                {
                    IsAlert = true;
                    AlertTitle = "Pending Time Off Requests";
                    AlertContent = "There are pending time off requests that need your attention.";
                }
                else
                {
                    AlertContent += "\nThere are pending time off requests that need your attention.";
                    AlertTitle += ", Pending Time Off Requests";
                }
            }
        }


        private double CountEmployeesMonthlyChangePercentage()
        {
            double previousMonthCount, currentMonthCount;

            if (user.Role.RoleName == "Admin")
            {
                previousMonthCount = _context.GetAllActiveEmployees()
                .Where(e => e.HireDate < DateOnly.FromDateTime(DateTime.Now.AddMonths(-1))).Count();

                currentMonthCount = _context.GetAllActiveEmployees().Count();

            }
            else if (user == null || user.Employee == null)
            {
                return 0;
            }
            else
            {
                previousMonthCount = _context.GetAllActiveEmployees()
                .Where(e => user.Employee.DepartmentId == e.DepartmentId && e.HireDate < DateOnly.FromDateTime(DateTime.Now.AddMonths(-1))).Count();

                currentMonthCount = _context.GetAllActiveEmployees().Where(e => user.Employee.DepartmentId == e.DepartmentId).Count();
            }


            return (currentMonthCount - previousMonthCount) / previousMonthCount * 100;
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
                    .Where(a => a.Employee.DepartmentId == user.Employee.DepartmentId && a.CheckInTime.HasValue && (a.CheckInTime.Value.Date == DateTime.Now.Date || !a.CheckOutTime.HasValue || a.CheckOutTime.Value.Date == DateTime.Now.Date)).Count();

                return presentEmployees;
            }
        }

        private int CountTotalEmployees()
        {
            if (user.Role.RoleName == "Admin")
            {
                return _context.GetAllActiveEmployees().Count();
            }
            else if (user == null || user.Employee == null)
            {
                return 0;
            }
            else
            {
                return _context.GetAllActiveEmployees()
                    .Where(e => e.DepartmentId == user.Employee.DepartmentId).Count();
            }
        }

        private int CountPendingTimeOffRequests()
        {
            if (user.Role.RoleName == "Admin")
            {
                return _context.GetAllTimeOffRequests()
                    .Where(r => r.Status == "Pending" && r.StartDate <= DateOnly.FromDateTime(DateTime.Now.AddDays(30))).Count();
            }
            else if (user == null || user.Employee == null || user.Employee.Departments.Count == 0)
            {
                return 0;
            }
            else
            {
                return _context.GetAllTimeOffRequests()
                .Where(r => r.Employee.DepartmentId == user.Employee.DepartmentId && r.Status == "Pending" && r.StartDate <= DateOnly.FromDateTime(DateTime.Now.AddDays(30))).Count();
            }
        }

        private int CountUpcomingReviews()
        {
            if (user.Role.RoleName == "Admin")
            {
                return _context.GetAllPerformanceReviews()
                    .Where(r => r.Status == "Pending" && r.ReviewDate <= DateOnly.FromDateTime(DateTime.Now.AddDays(30))).Count();
            }
            else if (user == null || user.Employee == null || user.Employee.Departments.Count == 0)
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
            var formWindow = new Views.EmployeeFormWindow(user);
            var viewModel = new EmployeeFormViewModel(user);
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
            _window.CurrentPage = new TimeOffRequestsPage(user);

        }
        [RelayCommand]
        private void ScheduleReview()
        {
            // Implementation for navigating to schedule review
            var formWindow = new PerformanceReviewFormWindow(user);
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