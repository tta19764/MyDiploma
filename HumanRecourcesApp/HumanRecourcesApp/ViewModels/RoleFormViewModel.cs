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
        private readonly User user;
        [ObservableProperty] private Role role;
        [ObservableProperty] private bool isEditMode;
        private string _roleName = string.Empty;
        [ObservableProperty] private ObservableCollection<PermissionCheckBoxItem> permissionsList;
        [ObservableProperty] private string errorMessage = string.Empty;
        [ObservableProperty] private bool isErrorVisible = false;

        public string RoleName
        {
            get => _roleName;
            set
            {
                SetProperty(ref _roleName, value);
                if (string.IsNullOrWhiteSpace(value))
                {
                    ErrorMessage = "Role name cannot be empty.";
                    IsErrorVisible = true; // Show error message
                    CanSave = false; // Disable save button
                }
                else
                {
                    ErrorMessage = string.Empty;
                    Role.RoleName = value;
                    IsErrorVisible = false; // Hide error message
                    CanSave = true; // Enable save button
                }
            }
        }

        [ObservableProperty] private string description;

        public string WindowTitle => IsEditMode ? "Edit Role" : "Create New Role";

        [ObservableProperty] private bool canSave = false;

        public RoleFormViewModel(User _user)
        {
            ErrorMessage = string.Empty;
            PermissionsList = new ObservableCollection<PermissionCheckBoxItem>();
            _context = new HumanResourcesDB();
            user = _user;
            Role = new Role { CreatedAt = DateTime.Now };
            IsEditMode = false;

            // Initialize properties
            RoleName = string.Empty;
            Description = string.Empty;

            // Load all permissions
            LoadPermissions();
        }

        public RoleFormViewModel(User _user, Role role)
        {
            _context = new HumanResourcesDB();
            user = _user;
            ErrorMessage = string.Empty;
            PermissionsList = new ObservableCollection<PermissionCheckBoxItem>();
            Role = new Role();
            Description = string.Empty;
            try
            {
                if (_context.GetRoleById(role.RoleId) == null)
                {
                    throw new Exception("Role not found");
                }
                Role = _context.GetRoleById(role.RoleId) ?? role;
                IsEditMode = true;

                // Initialize properties
                RoleName = Role.RoleName;
                Description = Role.Description ?? string.Empty;

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

                Role.Description = Description;

                // Save role
                if (IsEditMode)
                {
                    _context.UpdateRole(user, Role);
                }
                else
                {
                    _context.CreateRole(user, Role);
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