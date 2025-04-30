using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumanResourcesApp.DBClasses;
using HumanResourcesApp.Models;
using HumanResourcesApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace HumanResourcesApp.ViewModels
{
    public partial class TimeOffRequestsPageViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;

        private ObservableCollection<TimeOffRequestDisplayModel> timeOffRequests;
        public ObservableCollection<TimeOffRequestDisplayModel> TimeOffRequests
        {
            get => timeOffRequests;
            set => SetProperty(ref timeOffRequests, value);
        }

        [ObservableProperty]
        public ListCollectionView timeOffRequestsView;


        public TimeOffRequestsPageViewModel()
        {
            _context = new HumanResourcesDB();
            TimeOffRequests = new ObservableCollection<TimeOffRequestDisplayModel>();
            LoadRequests();
        }

        private void LoadRequests()
        {
            var requests = _context.GetAllTimeOffRequests();

            TimeOffRequests = new ObservableCollection<TimeOffRequestDisplayModel>(
                requests.Select(r => new TimeOffRequestDisplayModel
                {
                    TimeOffRequestId = r.TimeOffRequestId,
                    EmployeeName = $"{r.Employee.FirstName} {r.Employee.LastName}",
                    TimeOffTypeName = r.TimeOffType.TimeOffTypeName,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    TotalDays = r.TotalDays,
                    Status = r.Status,
                    Reason = r.Reason,
                    Comments = r.Comments,
                    ApprovedByName = r.ApprovedByNavigation != null ? $"{r.ApprovedByNavigation.FirstName} {r.ApprovedByNavigation.LastName}" : null,
                    ApprovalDate = r.ApprovalDate
                })
            );

            // Always create new ListCollectionView after loading
            TimeOffRequestsView = new ListCollectionView(TimeOffRequests);
            TimeOffRequestsView.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(TimeOffRequestDisplayModel.StatusPriority), System.ComponentModel.ListSortDirection.Ascending));
            TimeOffRequestsView.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(TimeOffRequestDisplayModel.StartDate), System.ComponentModel.ListSortDirection.Ascending));
            TimeOffRequestsView.IsLiveSorting = true;
        }


        [RelayCommand]
        private void Refresh() => LoadRequests();

        [RelayCommand]
        private void Create(Window window)
        {
            var form = new TimeOffRequestFormWindow();
            if (form.ShowDialog() == true)
                LoadRequests();
        }

        [RelayCommand]
        private void Review(TimeOffRequestDisplayModel timeOffRequest)
        {
            if (timeOffRequest == null)
                return;

            var request = _context.GetTimeOffRequestById(timeOffRequest.TimeOffRequestId);
            if (request == null)
            {
                MessageBox.Show("The selected time-off request could not be found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var formWindow = new TimeOffRequestReviewFormWindow();
            var viewModel = new TimeOffRequestReviewFormViewModel(request);
            formWindow.DataContext = viewModel;

            viewModel.RequestClose += (sender, result) =>
            {
                formWindow.DialogResult = result;
                formWindow.Close();
            };

            bool? result = formWindow.ShowDialog();
            if (result == true)
            {
                LoadRequests();
            }
        }

        [RelayCommand]
        private void Edit(TimeOffRequestDisplayModel requestDisplay)
        {
            var request = _context.GetTimeOffRequestById(requestDisplay.TimeOffRequestId);

            if (request != null)
            {
                var form = new TimeOffRequestFormWindow(request);
                if (form.ShowDialog() == true)
                    LoadRequests();
            }
        }

        [RelayCommand]
        private void Delete(TimeOffRequestDisplayModel requestDisplay)
        {
            if (MessageBox.Show("Are you sure you want to delete this request?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var request = _context.GetTimeOffRequestById(requestDisplay.TimeOffRequestId);
                if (request != null)
                {
                    _context.DeleteTimeOffRequest(request);
                    LoadRequests();
                }
            }
        }

    }
}
