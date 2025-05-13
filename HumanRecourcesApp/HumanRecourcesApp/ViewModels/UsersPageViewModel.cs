using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumanResourcesApp.Models;
using HumanResourcesApp.DBClasses;
using HumanResourcesApp.Views;
using System.Data;

namespace HumanResourcesApp.ViewModels
{
    public partial class UsersPageViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;

        [ObservableProperty] private ObservableCollection<User> users;
        private readonly User _user;
        [ObservableProperty] private User? selectedUser;
        [ObservableProperty] private bool canManageUsers = false;

        public UsersPageViewModel(User _user)
        {
            _context = new HumanResourcesDB();
            Users = new ObservableCollection<User>();
            this._user = _user;
            CanManageUsers = _context.HasPermission(_user, "ManageUsers");
            // Load users when the ViewModel is constructed
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                // Clear the existing users collection
                Users.Clear();
                var usersList = _context.GetAllUsers();
                foreach (var user in usersList)
                {
                    Users.Add(user);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void RefreshUsers()
        {
            LoadUsers();
        }

        [RelayCommand]
        private void AddUser()
        {
            try
            {
                var window = new UserForm(_user);

                if (window.ShowDialog() == true)
                {
                    LoadUsers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void EditUser(User user)
        {
            try
            {
                if (user == null) throw new Exception("User was not found");

                var window = new UserForm(_user, user);

                if (window.ShowDialog() == true)
                {
                    LoadUsers();
                }
            }
            catch (Exception ex)
            {
                _context.LogError(_user, "EditUser", ex);
            }
        }

        [RelayCommand]
        private void DeleteUser(User user)
        { 
            try
            {
                if (user == null) throw new Exception("User was not found");
                // Confirm deletion with the user
                var result = MessageBox.Show($"Are you sure you want to delete user {user.Username}?",
                    "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

                if (result)
                {
                    // Delete the user from the database
                    var userToDelete = _context.GetUserById(user.UserId);
                    if (userToDelete != null)
                    {
                        _context.DeleteUser(_user, userToDelete);

                        // Remove the user from the collection
                        Users.Remove(user);
                        SelectedUser = null;
                    }
                }
            }
            catch (Exception ex)
            {
                _context.LogError(_user, "DeleteUser", ex);
            }
        }
    }
}