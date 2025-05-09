using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumanResourcesApp.Classes;
using HumanResourcesApp.DBClasses;
using HumanResourcesApp.Models;
using System.Globalization;

namespace HumanResourcesApp.ViewModels
{
    public partial class PayrollItemsViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;
        private static readonly Regex _numericRegex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");

        [ObservableProperty] private ObservableCollection<PayrollItem> payrollItems;
        [ObservableProperty] private PayrollItem selectedPayrollItem;
        [ObservableProperty] private PayrollItem newPayrollItem;
        [ObservableProperty] private bool isAddingOrEditing;
        [ObservableProperty] private bool isEditing;
        [ObservableProperty] private string formTitle;
        [ObservableProperty] private string defaultValueText = "";
        private readonly User user;

        public PayrollItemsViewModel(User _user)
        {
            // Initialize context
            _context = new HumanResourcesDB();
            user = _user;

            // Initialize collections
            PayrollItems = new ObservableCollection<PayrollItem>();
            NewPayrollItem = new PayrollItem();
            SelectedPayrollItem = new PayrollItem();

            // Set default form title
            FormTitle = "Add Payroll Item";

            // Load data
            LoadPayrollItems();
        }

        private string _lastValidDefaultValueText = "";

        partial void OnDefaultValueTextChanged(string value)
        {
            // Only allow numeric values
            if (!string.IsNullOrEmpty(value) && !_numericRegex.IsMatch(value))
            {
                // Revert to the last valid value
                DefaultValueText = _lastValidDefaultValueText;
                return;
            }

            // Update the model when valid input is provided
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal defaultValue))
            {
                NewPayrollItem.DefaultValue = defaultValue;
                _lastValidDefaultValueText = value; // Store the valid value
            }
            else
            {
                NewPayrollItem.DefaultValue = null;
                if (string.IsNullOrEmpty(value))
                {
                    _lastValidDefaultValueText = ""; // Reset stored value if field is cleared
                }
            }
        }

        private void LoadPayrollItems()
        {
            PayrollItems.Clear();
            var itemsList = _context.GetAllPayrollItems();
            foreach (var item in itemsList)
            {
                PayrollItems.Add(item);
            }
        }

        [RelayCommand]
        private void AddPayrollItem()
        {
            IsAddingOrEditing = true;
            IsEditing = false;
            FormTitle = "Add Payroll Item";
            SelectedPayrollItem = new PayrollItem();

            // Set default values for new payroll item
            NewPayrollItem = new PayrollItem
            {
                ItemName = "",
                ItemType = "Earnings",
                IsPercentageBased = false,
                DefaultValue = 0,
                TaxableFlag = false,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            // Set the text representation of the default value
            DefaultValueText = NewPayrollItem.DefaultValue?.ToString() ?? "";
        }

        [RelayCommand]
        private void EditPayrollItem(PayrollItem item)
        {
            if (item == null) return;

            IsAddingOrEditing = true;
            IsEditing = true;
            FormTitle = "Edit Payroll Item";

            // Create a copy for editing
            NewPayrollItem = new PayrollItem
            {
                PayrollItemId = item.PayrollItemId,
                ItemName = item.ItemName,
                ItemType = item.ItemType,
                IsPercentageBased = item.IsPercentageBased,
                DefaultValue = item.DefaultValue,
                TaxableFlag = item.TaxableFlag,
                IsActive = item.IsActive,
                CreatedAt = item.CreatedAt
            };

            // Set the text representation of the default value
            DefaultValueText = NewPayrollItem.DefaultValue?.ToString("F2", CultureInfo.InvariantCulture) ?? "";
        }

        [RelayCommand]
        private void Save()
        {
            if (!IsAddingOrEditing) return;

            // Validate name
            if (string.IsNullOrWhiteSpace(NewPayrollItem.ItemName))
            {
                MessageBox.Show("Payroll item name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate item type
            if (string.IsNullOrWhiteSpace(NewPayrollItem.ItemType))
            {
                MessageBox.Show("Payroll item type is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate default value formatting
            if (!decimal.TryParse(DefaultValueText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal defaultValue))
            {
                MessageBox.Show("Default value must be a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Update the model with the parsed value
            NewPayrollItem.DefaultValue = defaultValue;

            try
            {
                if (IsEditing)
                {
                    _context.UpdatePayrollItem(user, NewPayrollItem);
                }
                else
                {
                    _context.CreatePayrollItem(user, NewPayrollItem);
                }

                LoadPayrollItems();
                IsAddingOrEditing = false;
            }
            catch (Exception ex)
            {
                _context.LogError(user, "SavePayrollItem", ex);
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            IsAddingOrEditing = false;
            NewPayrollItem = new PayrollItem();
            DefaultValueText = "";
        }

        [RelayCommand]
        private void DeletePayrollItem(PayrollItem item)
        {
            try
            {
                if (item == null) throw new Exception("No payroll item selected.");

                // Check if the payroll item has associated payroll details
                int detailsCount = item.PayrollDetails.Count;
                if (detailsCount > 0)
                {
                    var result = MessageBox.Show(
                        $"This payroll item has {detailsCount} associated payroll details. Deleting it will also delete all related payroll data.\n\nDo you want to continue?",
                        "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                else
                {
                    var result = MessageBox.Show($"Are you sure you want to delete the payroll item '{item.ItemName}'?",
                        "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                _context.DeletePayrollItem(user, item);
                PayrollItems.Remove(item);
            }
            catch (Exception ex)
            {
                _context.LogError(user, "DeletePayrollItem", ex);
            }
        }
    }
}