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
    /// Interaction logic for PerformanceReviewFormWindow.xaml
    /// </summary>
    public partial class PerformanceReviewFormWindow : Window
    {
        public PerformanceReviewFormWindow(User user)
        {
            InitializeComponent();
            DataContext = new ViewModels.PerformanceReviewFormViewModel(user);
        }
    }
}
