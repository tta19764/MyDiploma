﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumanResourcesApp.Classes;
using HumanResourcesApp.DBClasses;
using HumanResourcesApp.Models;

namespace HumanResourcesApp.ViewModels
{
    public partial class PayPeriodViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;

        [ObservableProperty] private ObservableCollection<PayPeriodDisplayModel> payPeriods;
        [ObservableProperty] private PayPeriodDisplayModel selectedPayPeriod;
        [ObservableProperty] private PayPeriodDisplayModel newPayPeriod;
        [ObservableProperty] private bool isAddingOrEditing;
        [ObservableProperty] private bool isEditing;
        [ObservableProperty] private string formTitle;
        private readonly User user;
        [ObservableProperty] private bool canProcessPayroll = false;

        public PayPeriodViewModel(User _user)
        {
            // Initialize context
            _context = new HumanResourcesDB();
            user = _user;
            CanProcessPayroll = _context.HasPermission(user, "ProcessPayroll");
            // Initialize collections
            PayPeriods = new ObservableCollection<PayPeriodDisplayModel>();
            NewPayPeriod = new PayPeriodDisplayModel();
            SelectedPayPeriod = new PayPeriodDisplayModel();

            // Set default form title
            FormTitle = "Add Pay Period";

            // Load data
            LoadPayPeriods();
        }

        private void LoadPayPeriods()
        {
            PayPeriods.Clear();
            var payPeriodList = _context.GetAllPayPeriods();
            foreach (var payPeriod in payPeriodList)
            {
                PayPeriods.Add(new PayPeriodDisplayModel
                {
                    PayPeriodId = payPeriod.PayPeriodId,
                    StartDate = payPeriod.StartDate,
                    EndDate = payPeriod.EndDate,
                    PaymentDate = payPeriod.PaymentDate,
                    Status = payPeriod.Status,
                    CreatedAt = payPeriod.CreatedAt,
                    PayrollCount = payPeriod.EmployeePayrolls.Count,
                    IsEditable = payPeriod.Status == "Draft",
                    IsDeletable = (payPeriod.Status != "Completed") || (payPeriod.Status == "Completed" && payPeriod.EmployeePayrolls.Count == 0)
                });
            }
        }

        [RelayCommand]
        private void AddPayPeriod()
        {
            IsAddingOrEditing = true;
            IsEditing = false;
            FormTitle = "Add Pay Period";
            SelectedPayPeriod = new PayPeriodDisplayModel();

            // Set default values for new pay period
            NewPayPeriod = new PayPeriodDisplayModel
            {
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(13)),  // Default to bi-weekly
                PaymentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(13)),
                Status = "Draft",
                CreatedAt = DateTime.Now,
                IsEditable = true
            };
        }

        [RelayCommand]
        private void EditPayPeriod(PayPeriodDisplayModel payPeriod)
        {
            try
            {
                if (payPeriod == null) throw new Exception("Pay period not found.");

                // Check if the payPeriod is editable (Draft status)
                if (!payPeriod.IsEditable)
                {
                    MessageBox.Show("Only pay periods with 'Draft' status can be edited.",
                        "Cannot Edit", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                IsAddingOrEditing = true;
                IsEditing = true;
                FormTitle = "Edit Pay Period";

                // Create a copy for editing
                NewPayPeriod = new PayPeriodDisplayModel
                {
                    PayPeriodId = payPeriod.PayPeriodId,
                    StartDate = payPeriod.StartDate,
                    EndDate = payPeriod.EndDate,
                    PaymentDate = payPeriod.PaymentDate,
                    Status = payPeriod.Status,
                    CreatedAt = payPeriod.CreatedAt,
                    PayrollCount = payPeriod.PayrollCount,
                    IsEditable = payPeriod.IsEditable
                };
            }
            catch (Exception ex)
            {
                _context.LogError(user, "EditPayPeriod", ex);
            }
        }

        [RelayCommand]
        private void Save()
        {
            if (!IsAddingOrEditing) return;

            // Validate start and end dates
            if (NewPayPeriod.EndDate < NewPayPeriod.StartDate)
            {
                MessageBox.Show("End date must be after start date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate payment date
            if (NewPayPeriod.PaymentDate < NewPayPeriod.EndDate)
            {
                MessageBox.Show("Payment date should be after the pay period end date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Create or update the pay period record
                var payPeriod = new PayPeriod
                {
                    PayPeriodId = NewPayPeriod.PayPeriodId,
                    StartDate = NewPayPeriod.StartDate,
                    EndDate = NewPayPeriod.EndDate,
                    PaymentDate = NewPayPeriod.PaymentDate,
                    Status = NewPayPeriod.Status,
                    CreatedAt = IsEditing ? NewPayPeriod.CreatedAt : DateTime.Now
                };

                if (IsEditing)
                {
                    // Double-check that we're only editing Draft status pay periods
                    var existingPayPeriod = _context.GetPayPeriodById(payPeriod.PayPeriodId);
                    if (existingPayPeriod != null && existingPayPeriod.Status != "Draft")
                    {
                        MessageBox.Show("Only pay periods with 'Draft' status can be edited.",
                            "Cannot Edit", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    _context.UpdatePayPeriod(user, payPeriod);
                }
                else
                {
                    // Check for overlapping periods
                    if (!_context.IsPayPeriodTimeValid(payPeriod))
                    {
                        MessageBox.Show("This pay period overlaps with an existing pay period.\n\nPlease adjust the start or end dates.",
                            "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    _context.CreatePayPeriod(user, payPeriod);
                }

                LoadPayPeriods();
                IsAddingOrEditing = false;
            }
            catch (Exception ex)
            {
                _context.LogError(user, "SavePayPeriod", ex);
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            IsAddingOrEditing = false;
            NewPayPeriod = new PayPeriodDisplayModel();
        }

        [RelayCommand]
        private void DeletePayPeriod(PayPeriodDisplayModel payPeriod)
        {
            try
            {
                if (payPeriod == null) throw new Exception("Pay period not found.");

                // Check if the payPeriod is editable (Draft status)
                if (!payPeriod.IsDeletable)
                {
                    MessageBox.Show("Only pay periods that don't have 'Completed' status can be deleted.",
                        "Cannot Delete", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Check if period has associated payrolls
                if (payPeriod.PayrollCount > 0)
                {
                    var result = MessageBox.Show(
                        $"This pay period has {payPeriod.PayrollCount} associated payroll records. Deleting it will also delete all related payroll data.\n\nDo you want to     continue?",
                        "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                else
                {
                    var result = MessageBox.Show($"Are you sure you want to delete this pay period?",
                        "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }

            
                PayPeriod? payPeriodToDelete = _context.GetPayPeriodById(payPeriod.PayPeriodId);
                if (payPeriodToDelete != null)
                {
                    _context.DeletePayPeriod(user, payPeriodToDelete);
                    PayPeriods.Remove(payPeriod);
                }
                else
                {
                    throw new Exception("Pay period not found.");
                }
            }
            catch (Exception ex)
            {
                _context.LogError(user, "DeletePayPeriod", ex);
            }
        }
    }
}