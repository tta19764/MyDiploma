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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HumanResourcesApp.Views
{
    /// <summary>
    /// Interaction logic for PerformanceCriteriaPage.xaml
    /// </summary>
    public partial class PerformanceCriteriaPage : Page
    {
        public PerformanceCriteriaPage()
        {
            InitializeComponent();
            DataContext = new ViewModels.PerformanceCriteriaViewModel();
        }
    }
}
