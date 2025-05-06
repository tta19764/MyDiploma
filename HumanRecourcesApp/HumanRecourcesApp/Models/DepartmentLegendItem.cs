using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HumanResourcesApp.Models
{
    public partial class DepartmentLegendItem : ObservableObject
    {
        [ObservableProperty] private string departmentName = string.Empty;
        [ObservableProperty] private SolidColorBrush color = new SolidColorBrush(Colors.Transparent);
        [ObservableProperty] private int count;
        [ObservableProperty] private double percentage;
    }
}
