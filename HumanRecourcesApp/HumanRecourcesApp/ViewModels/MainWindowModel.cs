using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using HumanResourcesApp.DBClasses;
using HumanResourcesApp.Models;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumanResourcesApp.Views;

namespace HumanRecourcesApp.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private string _userFullName = string.Empty;
        private string _userRole = string.Empty;

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

        public string UserRole
        {
            get => _userRole;
            set
            {
                if (_userRole != value)
                {
                    _userRole = value;
                    OnPropertyChanged(nameof(UserRole));
                }
            }
        }

        private Page _currentPage;
        public Page CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        public ICommand NavigateCommand { get; }

        public MainWindowViewModel(User user)
        {
            if (user != null)
            {
                NavigateCommand = new RelayCommand<object>(ExecuteNavigate);
                UserFullName = $"{user.FirstName} {user.LastName}";
                UserRole = user.Role?.RoleName ?? "Unknown";
            }
        }

        private void ExecuteNavigate(object? parameter)
        {
            string pageName = parameter as string;

            switch (pageName)
            {
                case "Dashboard":
                    CurrentPage = new DashboardPage(UserFullName);
                    break;
                default:
                    CurrentPage = null;
                    break;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
