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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HumanResourcesApp.Views
{
    /// <summary>
    /// Interaction logic for PositionPage.xaml
    /// </summary>
    public partial class PositionPage : Page
    {
        public PositionPage(User user)
        {
            InitializeComponent();
            DataContext = new ViewModels.PositionViewModel(user);
        }
    }
}
