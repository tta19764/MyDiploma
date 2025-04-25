using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumanResourcesApp.DBClasses;
using HumanResourcesApp.Models;
using HumanResourcesApp.Views;

namespace HumanResourcesApp.ViewModels
{
    public partial class RolesPageViewModel : ObservableObject
    {

        [ObservableProperty]private ObservableCollection<RoleDisplayModel> roles;
        private readonly HumanResourcesDB _context;

        public RolesPageViewModel()
        {
            _context = new HumanResourcesDB();
            roles = new ObservableCollection<RoleDisplayModel>();
            LoadRoles();
        }

        private void LoadRoles()
        {
            var roles = _context.GetAllRoles();
            Roles = new ObservableCollection<RoleDisplayModel>(
                roles.Select(r => new RoleDisplayModel
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName,
                    Description = r.Description ?? string.Empty,
                    CreatedAt = r.CreatedAt,
                    PermissionCount = r.RolePermissions.Count
                }));
        }

        [RelayCommand]
        private void Create()
        {
            var viewModel = new RoleFormViewModel();
            var window = new RoleFormWindow { DataContext = viewModel };

            if (window.ShowDialog() == true)
            {
                LoadRoles();
            }
        }

        [RelayCommand]
        private void Edit(RoleDisplayModel roleDisplayModel)
        {
            if (roleDisplayModel == null) return;

            var role = _context.GetRoleById(roleDisplayModel.RoleId);
            if (role == null) return;

            var viewModel = new RoleFormViewModel(role);
            var window = new RoleFormWindow { DataContext = viewModel };

            if (window.ShowDialog() == true)
            {
                LoadRoles();
            }
        }

        [RelayCommand]
        private void Delete(RoleDisplayModel roleDisplayModel)
        {
            try
            {
                if (roleDisplayModel == null) return;

                if (_context.IsRoleUsedInUsers(roleDisplayModel.RoleId))
                {
                    MessageBox.Show("This role is currently in use and cannot be deleted.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Add confirmation dialog here if needed  
                var message = MessageBox.Show($"Are you sure you want to delete the role '{roleDisplayModel.RoleName}'?",
                    "Delete Role", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (message == MessageBoxResult.Yes)
                {
                    var role = _context.GetRoleById(roleDisplayModel.RoleId);
                    if (role != null)
                    {
                        _context.DeleteRole(role);
                    }
                    else
                    {
                        MessageBox.Show("The role could not be found.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoadRoles();
            }
        }
    }
}