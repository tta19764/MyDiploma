using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using HumanRecourcesApp.ViewModels;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HumanResourcesApp.Models;

namespace HumanResourcesApp.Views
{
    /// <summary>
    /// Interaction logic for DepartmentPage.xaml
    /// </summary>
    public partial class DepartmentPage : Page
    {
        public DepartmentPage(User user)
        {
            InitializeComponent();
            DataContext = new DepartmentViewModel(user);
        }
    }
}
