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

namespace HumanResourcesApp.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        // Properties for summary cards

        private string _userFullName = string.Empty;
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

        public int TotalEmployeesCount { get; set; }
        public double EmployeeChangePercentage { get; set; }
        public double AttendancePercentage { get; set; }
        public int AbsentEmployeesCount { get; set; }
        public int PendingTimeOffRequestsCount { get; set; }
        public int UpcomingReviewsCount { get; set; }
        public string CurrentDate => DateTime.Now.ToString("ddd, d MMMM yyyy");

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // Commands for quick actions
        public ICommand AddEmployeeCommand { get; private set; }
        public ICommand ApproveTimeOffCommand { get; private set; }
        public ICommand ScheduleReviewCommand { get; private set; }
        public ICommand ProcessPayrollCommand { get; private set; }

        public DashboardViewModel(string _userName)
        {
            if (_userName != string.Empty)
            {
                UserFullName = _userName;
            }

            // Initialize commands
            AddEmployeeCommand = new RelayCommand(ExecuteAddEmployee);
            ApproveTimeOffCommand = new RelayCommand(ExecuteApproveTimeOff);
            ScheduleReviewCommand = new RelayCommand(ExecuteScheduleReview);
            ProcessPayrollCommand = new RelayCommand(ExecuteProcessPayroll);

            // Load dashboard data
            LoadDashboardData();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void LoadDashboardData()
        {
            // In a real app, this would load from your services/repositories
            TotalEmployeesCount = 128;
            EmployeeChangePercentage = 3.2;
            AttendancePercentage = 92.5;
            AbsentEmployeesCount = 7;
            PendingTimeOffRequestsCount = 12;
            UpcomingReviewsCount = 8;
        }

        // Command execution methods
        private void ExecuteAddEmployee()
        {
            // Implementation for navigating to add employee page
        }

        private void ExecuteApproveTimeOff()
        {
            // Implementation for navigating to time off approvals
        }

        private void ExecuteScheduleReview()
        {
            // Implementation for navigating to schedule review
        }

        private void ExecuteProcessPayroll()
        {
            // Implementation for navigating to process payroll
        }
    }

    



}