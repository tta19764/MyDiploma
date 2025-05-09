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

        [ObservableProperty] private ObservableCollection<RoleDisplayModel> roles;
        private readonly HumanResourcesDB _context;
        private readonly User user;

        public RolesPageViewModel(User _user)
        {
            _context = new HumanResourcesDB();
            roles = new ObservableCollection<RoleDisplayModel>();
            user = _user;
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
            var viewModel = new RoleFormViewModel(user);
            var window = new RoleFormWindow { DataContext = viewModel };

            if (window.ShowDialog() == true)
            {
                LoadRoles();
            }
        }

        [RelayCommand]
        private void EditRole(RoleDisplayModel roleDisplayModel)
        {
            try
            {
                if (roleDisplayModel == null) throw new Exception("Role display model was not found");

                var role = _context.GetRoleById(roleDisplayModel.RoleId);
                if (role == null) throw new Exception("Role was not found");

                var viewModel = new RoleFormViewModel(user, role);
                var window = new RoleFormWindow { DataContext = viewModel };

                if (window.ShowDialog() == true)
                {
                    LoadRoles();
                }
            }
            catch (Exception ex)
            {
                _context.LogError(user, "EditRole", ex);
            }
        }

        [RelayCommand]
        private void DeleteRole(RoleDisplayModel roleDisplayModel)
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
                        _context.DeleteRole(user, role);
                    }
                    else
                    {
                        throw new Exception("Role not found");
                    }
                }
            }
            catch (Exception ex)
            {
                _context.LogError(user, "DeleteRole", ex);
            }
            finally
            {
                LoadRoles();
            }
        }
    }
}