using HumanResourcesApp.ViewModels;
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
using HumanResourcesApp.Classes;
using System.Globalization;
using HumanResourcesApp.Models;
using HumanRecourcesApp.ViewModels;

namespace HumanResourcesApp.Views
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        public DashboardPage(MainWindowViewModel mainVM, User user)
        {
            InitializeComponent();
            DataContext = new DashboardViewModel(mainVM, user);
        }
    }
}
