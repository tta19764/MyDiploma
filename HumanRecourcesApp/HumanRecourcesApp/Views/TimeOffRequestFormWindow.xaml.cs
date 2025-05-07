using HumanResourcesApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HumanResourcesApp.Views
{
    /// <summary>
    /// Interaction logic for TimeOffRequestFormWindow.xaml
    /// </summary>
    public partial class TimeOffRequestFormWindow : Window
    {
        public TimeOffRequestFormWindow(User user)
        {
            InitializeComponent();
            DataContext = new ViewModels.TimeOffRequestFormViewModel(user);
        }

        public TimeOffRequestFormWindow(User user, TimeOffRequest timeOffRequest)
        {
            InitializeComponent();
            DataContext = new ViewModels.TimeOffRequestFormViewModel(user, timeOffRequest);
        }
    }
}
