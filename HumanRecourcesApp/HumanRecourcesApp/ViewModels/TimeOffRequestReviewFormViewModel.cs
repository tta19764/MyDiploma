using System;
using System.Windows;
using System.Xml.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumanResourcesApp.DBClasses;
using HumanResourcesApp.Models;
using HumanResourcesApp.Views;

namespace HumanResourcesApp.ViewModels
{
    public partial class TimeOffRequestReviewFormViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;
        private readonly User user;
        public TimeOffRequest Request { get; }

        [ObservableProperty] private string formTitle;
        [ObservableProperty] private string employeeName;
        [ObservableProperty] private string timeOffTypeName;
        [ObservableProperty] private DateTime startDate;
        [ObservableProperty] private DateTime endDate;
        [ObservableProperty] private int totalDays;
        [ObservableProperty] private string reason;
        [ObservableProperty] private string comments;
        [ObservableProperty] private string validationMessage = string.Empty;
        public event EventHandler<bool>? RequestClose;

        public TimeOffRequestReviewFormViewModel(User _user, TimeOffRequest request)
        {
            _context = new HumanResourcesDB();
            user = _user;
            Request = request;

            FormTitle = $"Review Request #{request.TimeOffRequestId}";
            EmployeeName = $"{request.Employee.FirstName} {request.Employee.LastName}";
            TimeOffTypeName = request.TimeOffType.TimeOffTypeName;
            StartDate = request.StartDate.ToDateTime(TimeOnly.MinValue);
            EndDate = request.EndDate.ToDateTime(TimeOnly.MinValue);
            TotalDays = request.TotalDays;
            Reason = request.Reason ?? string.Empty;
            Comments = request.Comments ?? string.Empty;
        }

        [RelayCommand]
        private void Approve(Window window)
        {
            try
            {
                Request.Status = "Approved";
                Request.ApprovalDate = DateTime.Now;
                Request.ApprovedBy = 1; // TODO: Replace with logged-in approver ID
                Request.Comments = Comments;

                var balance = _context.GetTimeOffBalance(Request.EmployeeId, Request.TimeOffTypeId);
                if (balance != null)
                {
                    balance.UsedDays = (balance.UsedDays ?? 0) + Request.TotalDays;
                    balance.RemainingDays = balance.TotalDays - (balance.UsedDays ?? 0);
                    _context.UpdateTimeOffBalance(user, balance);
                }

                _context.UpdateTimeOffRequest(user, Request);

                CloseWindow(true);
            }
            catch (Exception ex)
            {
                ValidationMessage = $"Error approving request: {ex.Message}";
            }
        }

        [RelayCommand]
        private void Reject(Window window)
        {
            try
            {
                Request.Status = "Rejected";
                Request.ApprovalDate = DateTime.Now;
                Request.ApprovedBy = 1; // TODO: Replace with logged-in approver ID
                Request.Comments = Comments;

                _context.UpdateTimeOffRequest(user, Request);

                CloseWindow(true);
            }
            catch (Exception ex)
            {
                ValidationMessage = $"Error rejecting request: {ex.Message}";
            }
        }

        [RelayCommand]
        private void Cancel(Window window)
        {
            CloseWindow(false);
        }

        private void CloseWindow(bool result)
        {
            RequestClose?.Invoke(this, result);
        }
    }
}
