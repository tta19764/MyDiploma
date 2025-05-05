using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResourcesApp.Models
{
    public partial class PayrollDetailDisplayModel : ObservableObject
    {
        [ObservableProperty]
        private int payrollDetailId;

        [ObservableProperty]
        private int payrollId;

        [ObservableProperty]
        private int payrollItemId;

        [ObservableProperty]
        private string itemName;

        [ObservableProperty]
        private string itemType;

        [ObservableProperty]
        private bool isPercentageBased;

        [ObservableProperty]
        private decimal defaultValue;

        [ObservableProperty]
        private decimal amount;

        [ObservableProperty]
        private bool isNew;
    }
}
