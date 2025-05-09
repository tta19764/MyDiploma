using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumanResourcesApp.DBClasses;
using HumanResourcesApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HumanResourcesApp.ViewModels
{
    public partial class TimeOffRequestFormViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;
        private readonly bool _isEditMode;
        private readonly User user;

        [ObservableProperty] private string formTitle;
        [ObservableProperty] private ObservableCollection<Employee> employees;
        [ObservableProperty] private ObservableCollection<TimeOffType> timeOffTypes;
        [ObservableProperty] private Employee selectedEmployee;
        [ObservableProperty] private TimeOffType selectedTimeOffType;
        [ObservableProperty] private DateTime? startDate;
        [ObservableProperty] private DateTime? endDate;
        [ObservableProperty] private string? reason;
        [ObservableProperty] private string status = "Pending";
        [ObservableProperty] private string? comments;
        [ObservableProperty] private string validationMessage = string.Empty;

        public TimeOffRequest Request { get; }

        // Create mode
        public TimeOffRequestFormViewModel(User _user)
        {
            _context = new HumanResourcesDB();
            user = _user;
            Employees = new ObservableCollection<Employee>();
            TimeOffTypes = new ObservableCollection<TimeOffType>();
            SelectedEmployee = new Employee();
            SelectedTimeOffType = new TimeOffType();
            Request = new TimeOffRequest { CreatedAt = DateTime.Now };
            _isEditMode = false;

            FormTitle = "New Time Off Request";
            LoadData();
        }

        // Edit mode
        public TimeOffRequestFormViewModel(User _user, TimeOffRequest request)
        {
            _context = new HumanResourcesDB();
            user = _user;
            Request = request;
            _isEditMode = true;

            // Initialize non-nullable fields to avoid CS8618 errors  
            Employees = new ObservableCollection<Employee>();
            TimeOffTypes = new ObservableCollection<TimeOffType>();
            SelectedEmployee = new Employee();
            SelectedTimeOffType = new TimeOffType();

            FormTitle = $"Edit Request #{request.TimeOffRequestId}";
            
            try
            {
                LoadData();
                SelectedEmployee = Employees.FirstOrDefault(e => e.EmployeeId == request.EmployeeId) ?? throw new InvalidOperationException("Employee not found in the list.");
                SelectedTimeOffType = TimeOffTypes.FirstOrDefault(t => t.TimeOffTypeId == request.TimeOffTypeId) ?? throw new InvalidOperationException("TimeOffType not found in the list.");
                StartDate = request.StartDate.ToDateTime(TimeOnly.MinValue);
                EndDate = request.EndDate.ToDateTime(TimeOnly.MinValue);
                Reason = request.Reason;
                Comments = request.Comments;
                Status = request.Status;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing TimeOffRequestFormViewModel: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadData()
        {
            Employees = new ObservableCollection<Employee>(_context.GetAllEmplyees());
            TimeOffTypes = new ObservableCollection<TimeOffType>(_context.GetAllActiveTimeOffTypes());
        }

        [RelayCommand]
        private void Save(Window window)
        {
            if (!ValidateForm()) return;

            try
            {
                Request.EmployeeId = SelectedEmployee.EmployeeId;
                Request.TimeOffTypeId = SelectedTimeOffType.TimeOffTypeId;
                Request.StartDate = DateOnly.FromDateTime(StartDate!.Value);
                Request.EndDate = DateOnly.FromDateTime(EndDate!.Value);
                Request.TotalDays = (Request.EndDate.DayNumber - Request.StartDate.DayNumber) + 1;
                Request.Reason = Reason;
                Request.Status = Status;
                Request.Comments = Comments;
                Request.CreatedAt = DateTime.Now;

                var balance = _context.GetTimeOffBalance(Request.EmployeeId, Request.TimeOffTypeId);
                if (balance == null || Request.TotalDays > (balance.RemainingDays ?? 0))
                {
                    ValidationMessage = "Insufficient time off balance.";
                    return;
                }

                if (_isEditMode)
                    _context.UpdateTimeOffRequest(user, Request);
                else
                    _context.AddTimeOffRequest(user, Request);

                window.DialogResult = true;
                window.Close();
            }
            catch (Exception ex)
            {
                ValidationMessage = $"Error saving request: {ex.Message}";
            }
        }

        private bool ValidateForm()
        {
            ValidationMessage = string.Empty;

            if (SelectedEmployee == null)
            {
                ValidationMessage = "Please select an employee.";
                return false;
            }

            if (SelectedTimeOffType == null)
            {
                ValidationMessage = "Please select a time off type.";
                return false;
            }

            if (!StartDate.HasValue || !EndDate.HasValue)
            {
                ValidationMessage = "Start and end dates are required.";
                return false;
            }

            if (StartDate > EndDate)
            {
                ValidationMessage = "Start date cannot be after end date.";
                return false;
            }

            return true;
        }

        [RelayCommand]
        private void Cancel(Window window)
        {
            if (window != null)
            {
                window.DialogResult = false;
                window.Close();
            }
        }

    }
}
