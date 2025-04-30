using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResourcesApp.Models
{
    public partial class PayPeriodDisplayModel : ObservableObject
    {
        [ObservableProperty] private int payPeriodId;
        [ObservableProperty] private DateOnly startDate;
        [ObservableProperty] private DateOnly endDate;
        [ObservableProperty] private DateOnly paymentDate;
        [ObservableProperty] private string status = string.Empty;
        public DateTime? CreatedAt { get; set; }
        [ObservableProperty] private int payrollCount;
        [ObservableProperty] private bool isEditable;
    }
}
