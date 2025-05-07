using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumanResourcesApp.Models;
using HumanResourcesApp.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;
using HumanResourcesApp.DBClasses;

namespace HumanRecourcesApp.ViewModels
{
    public partial class PayrollDetailsWindowViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;

        private readonly int employeeId;
        private readonly int payPeriodId;

        [ObservableProperty]
        private int payrollId;

        [ObservableProperty]
        private string employeeName;

        [ObservableProperty]
        private string position;

        [ObservableProperty]
        private decimal baseSalary;

        [ObservableProperty]
        private string payPeriodName;

        [ObservableProperty]
        private DateOnly startDate;

        [ObservableProperty]
        private DateOnly endDate;

        [ObservableProperty]
        private DateOnly paymentDate;

        [ObservableProperty]
        private string status;

        [ObservableProperty]
        private decimal grossSalary;

        [ObservableProperty]
        private decimal totalDeductions;

        [ObservableProperty]
        private decimal netSalary;

        [ObservableProperty]
        private ObservableCollection<PayrollDetailDisplayModel> earningsItems;

        [ObservableProperty]
        private ObservableCollection<PayrollDetailDisplayModel> benefitsAndAllowancesItems;

        [ObservableProperty]
        private ObservableCollection<PayrollDetailDisplayModel> deductionItems;

        [ObservableProperty]
        private ObservableCollection<PayrollDetailDisplayModel> taxItems;

        [ObservableProperty]
        private bool isActive;

        private readonly User user;

        public PayrollDetailsWindowViewModel(User _user, int payPeriodId, int employeeId)
        {
            _context = new HumanResourcesDB();
            user = _user;
            this.employeeId = employeeId;
            this.payPeriodId = payPeriodId;

            EarningsItems = new ObservableCollection<PayrollDetailDisplayModel>();
            BenefitsAndAllowancesItems = new ObservableCollection<PayrollDetailDisplayModel>();
            DeductionItems = new ObservableCollection<PayrollDetailDisplayModel>();
            TaxItems = new ObservableCollection<PayrollDetailDisplayModel>();

            PayrollId = 0;

            // Load data
            LoadData(payPeriodId, employeeId);

            // Create or load payroll details
            LoadOrCreatePayrollDetails();

            // Calculate totals
            CalculateTotals();

            // Subscribe to collection changes to update totals
            SubscribeToCollectionChanges();
        }

        private void SubscribeToCollectionChanges()
        {
            EarningsItems.CollectionChanged += (s, e) => CalculateTotals();
            BenefitsAndAllowancesItems.CollectionChanged += (s, e) => CalculateTotals();
            DeductionItems.CollectionChanged += (s, e) => CalculateTotals();
            TaxItems.CollectionChanged += (s, e) => CalculateTotals();

            // Subscribe to property changed events for each item
            foreach (var item in EarningsItems.Concat(BenefitsAndAllowancesItems)
            .Concat(DeductionItems).Concat(TaxItems))
            {
                item.PropertyChanged += PayrollItem_PropertyChanged;
            }
        }

        private void PayrollItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PayrollDetailDisplayModel.Amount))
            {
                CalculateTotals();
            }
        }

        private void LoadData(int payPeriodId, int employeeId)
        {
            // Load payroll data
            var payPeriod = _context.GetPayPeriodById(payPeriodId);

            if (payPeriod == null)
            {
                MessageBox.Show("Pay period data not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CloseDialogWithResult(false);
                return;
            }

            // Load employee data
            var employee = _context.GetEmployeeById(employeeId);

            if (employee == null)
            {
                MessageBox.Show("Employee data not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CloseDialogWithResult(false);
                return;
            }

            if(_context.GetEmployeePayrollByEmployeeAndPayPeriodId(payPeriodId, employeeId) != null)
            {
                PayrollId = _context.GetEmployeePayrollByEmployeeAndPayPeriodId(payPeriodId, employeeId).PayrollId;
            }
            else
            {
                PayrollId = 0;
            }

            // Set employee properties
            EmployeeName = $"{employee.FirstName} {employee.LastName}";
            Position = employee.Position.PositionTitle;
            BaseSalary = employee.Salary;

            // Set payroll period properties
            PayPeriodName = $"{payPeriod.StartDate:MMM dd} - {payPeriod.EndDate:MMM dd, yyyy}";
            StartDate = payPeriod.StartDate;
            EndDate = payPeriod.EndDate;
            PaymentDate = payPeriod.PaymentDate;
            Status = payPeriod.Status;

            // Set IsActive flag based on payroll status
            IsActive = (Status == "Active");
        }

        private void LoadOrCreatePayrollDetails()
        {
            // Get existing payroll details if any
            var existingDetails = _context.GetPayrollDetailsByPayrollId(PayrollId);

            if (existingDetails != null && existingDetails.Any())
            {
                LoadExistingPayrollDetails(existingDetails);
            }
            else
            {
                CreateDefaultPayrollDetails();
            }
        }

        private void LoadExistingPayrollDetails(IEnumerable<PayrollDetail> details)
        {
            foreach (var detail in details)
            {
                var displayModel = new PayrollDetailDisplayModel
                {
                    PayrollDetailId = detail.PayrollDetailId,
                    PayrollId = detail.PayrollId,
                    PayrollItemId = detail.PayrollItemId,
                    ItemName = detail.PayrollItem.ItemName,
                    ItemType = detail.PayrollItem.ItemType,
                    IsPercentageBased = detail.PayrollItem.IsPercentageBased ?? false,
                    DefaultValue = detail.PayrollItem.DefaultValue ?? 0,
                    Amount = detail.Amount,
                    IsNew = false
                };

                AddToAppropriateCollection(displayModel);
            }
        }

        private void CreateDefaultPayrollDetails()
        {
            // Get all active payroll items
            var payrollItems = _context.GetAllPayrollItems()
                .Where(item => item.IsActive == true)
                .OrderBy(item =>
                    item.ItemType == "Earnings" ||
                    item.ItemType == "Benefit" ||
                    item.ItemType == "Allowance" ? 0 : 1)
                .ToList();


            foreach (var item in payrollItems)
            {
                decimal amount = 0;

                // Calculate amount based on item type and settings
                if (item.ItemType == "Earnings" || item.ItemType == "Benefit" || item.ItemType == "Allowance")
                {
                    if (item.ItemName == "Basic Salary")
                    {
                        amount = BaseSalary;
                    }
                    else if (item.IsPercentageBased == true && item.DefaultValue.HasValue)
                    {
                        amount = BaseSalary * (item.DefaultValue.Value / 100);
                    }
                    else if (item.DefaultValue.HasValue)
                    {
                        amount = item.DefaultValue.Value;
                    }
                }
                else if (item.ItemType == "Deduction" || item.ItemType == "Tax")
                {
                    // For deductions and taxes, we need to calculate gross pay first
                    decimal calculatedGrossPay = CalculateGrossPay();

                    if (item.IsPercentageBased == true && item.DefaultValue.HasValue)
                    {
                        amount = calculatedGrossPay * (item.DefaultValue.Value / 100);
                    }
                    else if (item.DefaultValue.HasValue)
                    {
                        amount = item.DefaultValue.Value;
                    }
                }

                // Create a display model for each item, even if amount is zero
                var displayModel = new PayrollDetailDisplayModel
                {
                    PayrollDetailId = 0, // Will be assigned on save
                    PayrollId = PayrollId,
                    PayrollItemId = item.PayrollItemId,
                    ItemName = item.ItemName,
                    ItemType = item.ItemType,
                    IsPercentageBased = item.IsPercentageBased ?? false,
                    DefaultValue = item.DefaultValue ?? 0,
                    Amount = amount,
                    IsNew = true
                };

                AddToAppropriateCollection(displayModel);
            }
        }

        private decimal CalculateGrossPay()
        {
            // Calculate gross pay from earnings items
            decimal grossPay = BaseSalary; // Start with base salary

            // Add other earnings if any exist in the collections
            foreach (var item in EarningsItems)
            {
                grossPay += item.Amount;
            }

            // Add benefits and allowances
            foreach (var item in BenefitsAndAllowancesItems)
            {
                grossPay += item.Amount;
            }

            return grossPay;
        }

        private void AddToAppropriateCollection(PayrollDetailDisplayModel model)
        {
            switch (model.ItemType)
            {
                case "Earnings":
                    EarningsItems.Add(model);
                    break;
                case "Benefit":
                case "Allowance":
                    BenefitsAndAllowancesItems.Add(model);
                    break;
                case "Deduction":
                    DeductionItems.Add(model);
                    break;
                case "Tax":
                    TaxItems.Add(model);
                    break;
            }
        }

        private void CalculateTotals()
        {
            // Calculate gross salary
            GrossSalary = 0;

            // Add earnings
            foreach (var item in EarningsItems)
            {
                GrossSalary += item.Amount;
            }

            // Add benefits and allowances
            foreach (var item in BenefitsAndAllowancesItems)
            {
                GrossSalary += item.Amount;
            }


            // Calculate deductions
            foreach (var item in DeductionItems)
            {
                item.Amount = item.IsPercentageBased ? (GrossSalary * (item.DefaultValue / 100)) : item.Amount;
            }

            // Calculate taxes
            foreach (var item in TaxItems)
            {
                item.Amount = item.IsPercentageBased ? (GrossSalary * (item.DefaultValue / 100)) : item.Amount;
            }


            // Calculate deductions
            TotalDeductions = 0;

            // Add deductions
            foreach (var item in DeductionItems)
            {
                TotalDeductions += item.Amount;
            }

            // Add taxes
            foreach (var item in TaxItems)
            {
                TotalDeductions += item.Amount;
            }

            // Calculate net salary
            NetSalary = GrossSalary - TotalDeductions;
        }

        [RelayCommand]
        private void Close()
        {
            CloseDialogWithResult(false);
        }


        [RelayCommand]
        private void Save()
        {
            try
            {
                // Get all details from collections
                var allDetails = EarningsItems
                    .Concat(BenefitsAndAllowancesItems)
                    .Concat(DeductionItems)
                .Concat(TaxItems)
                    .ToList();

                if (PayrollId != 0)
                {
                    // Update existing payroll
                    var existingPayroll = _context.GetEmployeePayrollById(PayrollId);
                    if (existingPayroll != null)
                    {
                        existingPayroll.GrossSalary = GrossSalary;
                        existingPayroll.TotalDeductions = TotalDeductions;
                        existingPayroll.NetSalary = NetSalary;
                        _context.UpdateEmployeePayroll(existingPayroll);
                    }
                }
                else
                {
                    // Create new payroll
                    var newPayroll = new EmployeePayroll
                    {
                        EmployeeId = employeeId,
                        PayPeriodId = payPeriodId,
                        BaseSalary = BaseSalary,
                        GrossSalary = GrossSalary,
                        TotalDeductions = TotalDeductions,
                        NetSalary = NetSalary,
                        CreatedAt = DateTime.Now
                    };
                    PayrollId = _context.CreateEmployeePayrollReturnId(user, newPayroll);
                } 

                // Handle new and existing details
                foreach (var detail in allDetails)
                {
                    if (detail.IsNew)
                    {
                        // Only create if amount is not zero
                        if (detail.Amount != 0)
                        {
                            var newDetail = new PayrollDetail
                            {
                                PayrollId = PayrollId,
                                PayrollItemId = detail.PayrollItemId,
                                Amount = detail.Amount,
                                CreatedAt = DateTime.Now
                            };
                            _context.CreatePayrollDetail(user, newDetail);
                        }
                    }
                    else
                    {
                        // Update existing detail
                        var existingDetail = new PayrollDetail
                        {
                            PayrollDetailId = detail.PayrollDetailId,
                            PayrollId = detail.PayrollId,
                            PayrollItemId = detail.PayrollItemId,
                            Amount = detail.Amount
                        };
                        _context.UpdatePayrollDetail(existingDetail);
                    }
                }

                MessageBox.Show("Payroll details saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseDialogWithResult(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving payroll details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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