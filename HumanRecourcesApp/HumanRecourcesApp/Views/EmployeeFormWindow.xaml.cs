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
    /// Interaction logic for EmployeeFormWindow.xaml
    /// </summary>
    public partial class EmployeeFormWindow : Window
    {
        public EmployeeFormWindow()
        {
            InitializeComponent();
            DataContext = new ViewModels.EmployeeFormViewModel();
        }
    }
}
