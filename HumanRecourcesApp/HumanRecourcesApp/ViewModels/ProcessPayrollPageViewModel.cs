using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumanRecourcesApp.ViewModels;
using HumanResourcesApp.DBClasses;
using HumanResourcesApp.Models;
using HumanResourcesApp.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;

namespace HumanResourcesApp.ViewModels
{
    public partial class ProcessPayrollPageViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;
        private readonly MainWindowViewModel _window;
        private readonly User user;

        [ObservableProperty]
        private ObservableCollection<PayPeriodDisplayModel> payPeriods;

        [ObservableProperty]
        private ObservableCollection<PayrollEmployeeDisplayModel> payrollEmployees;

        [ObservableProperty]
        private ObservableCollection<PayrollEmployeeDisplayModel> displayPayrollEmployees;

        [ObservableProperty]
        private PayrollEmployeeDisplayModel selectedEmployee;

        private PayPeriodDisplayModel selectedPayPeriod;
        public PayPeriodDisplayModel SelectedPayPeriod
        {
            get => selectedPayPeriod;
            set
            {
                SetProperty(ref selectedPayPeriod, value);
                LoadSelectedPayPeriodDetails();
            }
        }

        [ObservableProperty]
        private string selectedPayPeriodStatus;

        [ObservableProperty]
        private DateOnly selectedPayPeriodStartDate;

        [ObservableProperty]
        private DateOnly selectedPayPeriodEndDate;

        [ObservableProperty]
        private DateOnly selectedPayPeriodPaymentDate;

        [ObservableProperty]
        private bool isPayPeriodSelected;

        [ObservableProperty]
        private bool canProcessPayroll;

        private string searchText;
        public string SearchText
        {
            get => searchText;
            set
            {
                SetProperty(ref searchText, value);
                FilterEmployees();
            }
        }

        public ProcessPayrollPageViewModel(MainWindowViewModel window, User _user)
        {
            _context = new HumanResourcesDB();
            _window = window;
            user = _user;

            displayPayrollEmployees = new ObservableCollection<PayrollEmployeeDisplayModel>();
            PayPeriods = new ObservableCollection<PayPeriodDisplayModel>();
            PayrollEmployees = new ObservableCollection<PayrollEmployeeDisplayModel>();

            LoadPayPeriods();            
        }

        private void LoadPayPeriods()
        {
            var periods = _context.GetAllPayPeriods();
            PayPeriods.Clear();

            foreach (var period in periods)
            {
                PayPeriods.Add(new PayPeriodDisplayModel()
                {
                    PayPeriodId = period.PayPeriodId,
                    StartDate = period.StartDate,
                    EndDate = period.EndDate,
                    PaymentDate = period.PaymentDate,
                    Status = period.Status,
                    CreatedAt = period.CreatedAt,
                    PayrollCount = period.EmployeePayrolls.Count,
                    IsEditable = period.Status == "Draft"
                });
            }

            // Select the most recent open pay period if available
            var openPeriod = PayPeriods.FirstOrDefault(p => p.Status == "Active");
            if (openPeriod != null)
            {
                SelectedPayPeriod = PayPeriods.FirstOrDefault(pp => pp.PayPeriodId == openPeriod.PayPeriodId);
            }
            else if (PayPeriods.Any())
            {
                // Otherwise, select the most recent pay period
                SelectedPayPeriod = PayPeriods.OrderByDescending(p => p.StartDate).First();
            }
        }

        private void LoadSelectedPayPeriodDetails()
        {
            var payPeriod = PayPeriods.FirstOrDefault(p => p.PayPeriodId == SelectedPayPeriod.PayPeriodId);
            if (payPeriod == null)
            {
                IsPayPeriodSelected = false;
                CanProcessPayroll = false;
                PayrollEmployees.Clear();
                return;
            }

            IsPayPeriodSelected = true;
            SelectedPayPeriodStatus = payPeriod.Status;
            SelectedPayPeriodStartDate = payPeriod.StartDate;
            SelectedPayPeriodEndDate = payPeriod.EndDate;
            SelectedPayPeriodPaymentDate = payPeriod.PaymentDate;
            CanProcessPayroll = payPeriod.Status == "Active";

            LoadEmployeePayroll();
        }

        private void LoadEmployeePayroll()
        {
            PayrollEmployees.Clear();

            // Get all employees
            var employees = _context.GetAllEmplyees();

            // Get existing payroll records for this period
            var payrollRecords = _context.GetAllEmployeePayrolls()
                .Where(p => p.PayPeriodId == SelectedPayPeriod.PayPeriodId)
                .ToList();

            foreach (var employee in employees)
            {
                var payrollRecord = payrollRecords.FirstOrDefault(p => p.EmployeeId == employee.EmployeeId);

                var employeeViewModel = new PayrollEmployeeDisplayModel
                {
                    EmployeeId = employee.EmployeeId,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Department = employee.Department,
                    Position = employee.Position,
                    Salary = employee.Salary
                };

                if (payrollRecord != null)
                {
                    employeeViewModel.PayrollId = payrollRecord.PayrollId;
                    employeeViewModel.BaseSalary = payrollRecord.BaseSalary;
                    employeeViewModel.GrossPay = payrollRecord.GrossSalary;
                    employeeViewModel.TotalDeductions = payrollRecord.TotalDeductions;
                    employeeViewModel.NetPay = payrollRecord.NetSalary;
                    employeeViewModel.PayrollStatus = payrollRecord.Status ?? string.Empty;
                    employeeViewModel.PayrollDetails = new ObservableCollection<PayrollDetail>(
                        _context.GetAllPayrollDetails().Where(pd => pd.PayrollId == payrollRecord.PayrollId));
                }
                else
                {
                    // No existing payroll record, set defaults
                    employeeViewModel.BaseSalary = employee.Salary;
                    employeeViewModel.GrossPay = employee.Salary + CalculateBonuses(employeeViewModel);
                    employeeViewModel.TotalDeductions = CalculateDeductions(employeeViewModel);
                    employeeViewModel.NetPay = employeeViewModel.GrossPay - employeeViewModel.TotalDeductions;
                    employeeViewModel.PayrollStatus = "Pending";
                    employeeViewModel.PayrollDetails = new ObservableCollection<PayrollDetail>();
                }

                PayrollEmployees.Add(employeeViewModel);
            }

            FilterEmployees();
        }

        private void FilterEmployees()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                // Show all employees
                foreach (var employee in PayrollEmployees)
                {
                    employee.IsVisible = true;
                }
                UpdateDisplayList();
                return;
            }

            string searchLower = SearchText.ToLower();

            foreach (var employee in PayrollEmployees)
            {
                // Check if employee name or ID matches the search text
                bool matches = employee.EmployeeId.ToString().Contains(searchLower) ||
                              employee.FullName.ToLower().Contains(searchLower);

                employee.IsVisible = matches;
            }
            UpdateDisplayList();
        }

        private void UpdateDisplayList()
        {
            // Update the display list
            DisplayPayrollEmployees.Clear();
            foreach (var employee in PayrollEmployees.Where(e => e.IsVisible))
            {
                DisplayPayrollEmployees.Add(employee);
            }
        }

        [RelayCommand]
        private void ClearSearch()
        {
            SearchText = string.Empty;
        }

        [RelayCommand]
        private void CreatePayPeriod()
        {
            _window.CurrentPage = new PayPeriodsPage(user);
        }

        [RelayCommand]
        private void ProcessPayroll()
        {
            if (!CanProcessPayroll || !IsPayPeriodSelected)
            {
                MessageBox.Show("Cannot process payroll for the selected pay period.", "Process Payroll",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Get the currently selected pay period
                var payPeriod = PayPeriods.First(p => p.PayPeriodId == SelectedPayPeriod.PayPeriodId);

                // Process payroll for each employee
                foreach (var employee in PayrollEmployees)
                {
                    if (employee.PayrollId == 0)
                    {
                        // Create new payroll record
                        var newPayroll = new EmployeePayroll
                        {
                            EmployeeId = employee.EmployeeId,
                            PayPeriodId = SelectedPayPeriod.PayPeriodId,
                            BaseSalary = employee.BaseSalary,
                            GrossSalary = employee.GrossPay,
                            TotalDeductions = CalculateDeductions(employee),
                            NetSalary = employee.GrossPay - CalculateDeductions(employee),
                            Status = "Processed",
                            ProcessedBy = user.UserId,
                            ProcessedDate = DateTime.Now,
                            CreatedAt = DateTime.Now
                        };

                        int payrollId = _context.CreateEmployeePayrollReturnId(user, newPayroll);

                        // Create payroll details
                        CreateDefaultPayrollDetails(payrollId, employee);
                    }
                    else
                    {
                        // Update existing payroll record
                        var existingPayroll = new EmployeePayroll
                        {
                            PayrollId = employee.PayrollId,
                            EmployeeId = employee.EmployeeId,
                            PayPeriodId = SelectedPayPeriod.PayPeriodId,
                            BaseSalary = employee.BaseSalary,
                            GrossSalary = employee.GrossPay,
                            TotalDeductions = employee.TotalDeductions,
                            NetSalary = employee.NetPay,
                            Status = "Processed",
                            ProcessedBy = user.UserId,
                            ProcessedDate = DateTime.Now
                        };

                        _context.UpdateEmployeePayroll(existingPayroll);
                    }
                }

                // Update pay period status
                if(payPeriod.Status == "Active")
                {
                    PayPeriod curPayPeriod = _context.GetPayPeriodById(payPeriod.PayPeriodId);
                    curPayPeriod.Status = "Completed";
                    _context.UpdatePayPeriod(curPayPeriod);
                }

                // Reload data
                LoadPayPeriods();

                MessageBox.Show("Payroll has been successfully processed.", "Process Payroll",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing payroll: {ex.Message}", "Process Payroll Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private decimal CalculateBonuses(PayrollEmployeeDisplayModel employee)
        {
            // Get standard deduction items from database
            var bonusItems = _context.GetAllPayrollItems()
                .Where(item => (item.ItemType != "Deduction" && item.ItemType != "Tax") && item.IsActive == true)
                .ToList();

            decimal totalBonuses = 0;

            foreach (var item in bonusItems)
            {
                decimal bonusAmount;

                if (item.IsPercentageBased == true && item.DefaultValue.HasValue)
                {
                    // Calculate percentage-based deduction
                    bonusAmount = employee.Salary * (item.DefaultValue.Value / 100);
                }
                else if (item.DefaultValue.HasValue)
                {
                    // Fixed amount deduction
                    bonusAmount = item.DefaultValue.Value;
                }
                else
                {
                    // Skip if no default value
                    continue;
                }

                totalBonuses += bonusAmount;
            }

            return totalBonuses;
        }

        private decimal CalculateDeductions(PayrollEmployeeDisplayModel employee)
        {
            // Get standard deduction items from database
            var deductionItems = _context.GetAllPayrollItems()
                .Where(item => (item.ItemType == "Deduction" || item.ItemType == "Tax") && item.IsActive == true)
                .ToList();

            decimal totalDeductions = 0;

            foreach (var item in deductionItems)
            {
                decimal deductionAmount;

                if (item.IsPercentageBased == true && item.DefaultValue.HasValue)
                {
                    // Calculate percentage-based deduction
                    deductionAmount = employee.GrossPay * (item.DefaultValue.Value / 100);
                }
                else if (item.DefaultValue.HasValue)
                {
                    // Fixed amount deduction
                    deductionAmount = item.DefaultValue.Value;
                }
                else
                {
                    // Skip if no default value
                    continue;
                }

                totalDeductions += deductionAmount;
            }

            return totalDeductions;
        }

        private void CreateDefaultPayrollDetails(int payrollId, PayrollEmployeeDisplayModel employee)
        {
            // Get all active payroll items
            var payrollItems = _context.GetAllPayrollItems()
                .Where(item => item.IsActive == true)
                .ToList();

            foreach (var item in payrollItems)
            {
                decimal amount = 0;

                // Calculate amount based on item type and settings
                if (item.ItemType == "Earnings" || item.ItemType == "Benefit" || item.ItemType == "Allowance")
                {
                    if (item.ItemName == "Basic Salary")
                    {
                        amount = employee.Salary;
                    }
                    else if (item.IsPercentageBased == true && item.DefaultValue.HasValue)
                    {
                        amount = employee.Salary * (item.DefaultValue.Value / 100);
                    }
                    else if (item.DefaultValue.HasValue)
                    {
                        amount = item.DefaultValue.Value;
                    }
                }
                else if (item.ItemType == "Deduction" || item.ItemType == "Tax")
                {
                    if (item.IsPercentageBased == true && item.DefaultValue.HasValue)
                    {
                        amount = employee.GrossPay * (item.DefaultValue.Value / 100);
                    }
                    else if (item.DefaultValue.HasValue)
                    {
                        amount = item.DefaultValue.Value;
                    }
                }

                // Only create detail if amount is non-zero
                if (amount != 0)
                {
                    var payrollDetail = new PayrollDetail
                    {
                        PayrollId = payrollId,
                        PayrollItemId = item.PayrollItemId,
                        Amount = amount,
                        CreatedAt = DateTime.Now
                    };

                    _context.CreatePayrollDetail(user, payrollDetail);
                }
            }
        }

        [RelayCommand]
        private void ViewPayrollDetails(PayrollEmployeeDisplayModel employee)
        {
            if (employee == null) return;

            var window = new PayrollDetailsWindow(user, SelectedPayPeriod.PayPeriodId, employee.EmployeeId);

            if(window.ShowDialog() == true)
            {
                // Refresh data after viewing details
                LoadEmployeePayroll();
            }
        }
    }
}