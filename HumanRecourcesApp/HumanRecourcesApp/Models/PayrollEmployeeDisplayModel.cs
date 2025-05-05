using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResourcesApp.Models
{
    public partial class PayrollEmployeeDisplayModel : ObservableObject
    {
        [ObservableProperty]
        private int employeeId;

        [ObservableProperty]
        private string firstName;

        [ObservableProperty]
        private string lastName;

        [ObservableProperty]
        private Department department;

        [ObservableProperty]
        private Position position;

        [ObservableProperty]
        private decimal salary;

        [ObservableProperty]
        private int payrollId;

        [ObservableProperty]
        private decimal baseSalary;

        [ObservableProperty]
        private decimal grossPay;

        [ObservableProperty]
        private decimal totalDeductions;

        [ObservableProperty]
        private decimal netPay;

        [ObservableProperty]
        private string payrollStatus;

        [ObservableProperty]
        private int? processedBy;

        [ObservableProperty]
        private DateTime? processedDate;

        [ObservableProperty]
        private string paymentReference;

        [ObservableProperty]
        private ObservableCollection<PayrollDetail> payrollDetails;

        [ObservableProperty]
        private bool isVisible = true;

        public string FullName => $"{FirstName} {LastName}";
    }
}
