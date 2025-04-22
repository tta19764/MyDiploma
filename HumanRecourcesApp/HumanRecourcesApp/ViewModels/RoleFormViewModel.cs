using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumanResourcesApp.DBClasses;
using HumanResourcesApp.Models;

namespace HumanResourcesApp.ViewModels
{
    public partial class RoleFormViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;
        [ObservableProperty]private Role role;
        [ObservableProperty]private bool isEditMode;
        private string _roleName;
        private string _description;
        [ObservableProperty]private ObservableCollection<PermissionCheckBoxItem> permissionsList;
        [ObservableProperty]private string errorMessage;

        public string RoleName
        {
            get => _roleName;
            set
            {
                SetProperty(ref _roleName, value);
                if (string.IsNullOrWhiteSpace(value))
                {
                    ErrorMessage = "Role name cannot be empty.";
                }
                else
                {
                    ErrorMessage = string.Empty;
                    Role.RoleName = value;
                }
                SetProperty(ref _roleName, value, nameof(CanSave)); // Notify that CanSave may have changed
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                SetProperty(ref _description, value);
                if(string.IsNullOrWhiteSpace(value))
                {
                    ErrorMessage = "Description cannot be empty.";
                }
                else
                {
                    ErrorMessage = string.Empty;
                    Role.Description = value;
                }
                SetProperty(ref _description, value, nameof(CanSave)); // Notify that CanSave may have changed
            }
        }

        public string WindowTitle => IsEditMode ? "Edit Role" : "Create New Role";

        public bool CanSave => !string.IsNullOrWhiteSpace(RoleName) && errorMessage == string.Empty;

        public RoleFormViewModel()
        {
            ErrorMessage = string.Empty;
            _context = new HumanResourcesDB();
            Role = new Role { CreatedAt = DateTime.Now };
            IsEditMode = false;

            // Initialize properties
            RoleName = string.Empty;
            Description = string.Empty;

            // Load all permissions
            LoadPermissions();
        }

        public RoleFormViewModel(Role role)
        {
            try
            {
                ErrorMessage = string.Empty;
                _context = new HumanResourcesDB();
                if (_context.GetRoleById(role.RoleId) == null)
                {
                    throw new Exception("Role not found");
                }
                Role = _context.GetRoleById(role.RoleId);
                IsEditMode = true;

                // Initialize properties
                RoleName = Role.RoleName;
                Description = Role.Description;

                // Load all permissions
                LoadPermissions();
            }
            catch (Exception ex) {
                MessageBox.Show($"An error occurred: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPermissions()
        {
            var allPermissions = _context.GetAllPermissions();

            PermissionsList = new ObservableCollection<PermissionCheckBoxItem>(
                allPermissions.Select(p => new PermissionCheckBoxItem
                {
                    PermissionId = p.PermissionId,
                    PermissionName = p.PermissionName,
                    Description = p.Description ?? string.Empty,
                    IsSelected = Role.RolePermissions
                                     .Any(rp => rp.PermissionId == p.PermissionId)
                }));
        }

        [RelayCommand]
        private void Save()
        {
            try
            {

                // Update role permissions
                Role.RolePermissions.Clear();

                foreach (var permissionItem in PermissionsList.Where(p => p.IsSelected))
                {
                    Role.RolePermissions.Add(new RolePermission
                    {
                        RoleId = Role.RoleId,
                        PermissionId = permissionItem.PermissionId
                    });
                }

                if (!_context.IsUniqueRoleName(Role))
                {
                    ErrorMessage = "Role name must be unique.";
                    return;
                }

                // Save role
                if (IsEditMode)
                {
                    _context.UpdateRole(Role);
                }
                else
                {
                    _context.CreateRole(Role);
                }

                CloseDialogWithResult(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseDialogWithResult(false);
        }

        private void CloseDialogWithResult(bool? result)
        {
            if (Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this) is Window window)
            {
                window.DialogResult = result;
                window.Close();
            }
        }
    }
}