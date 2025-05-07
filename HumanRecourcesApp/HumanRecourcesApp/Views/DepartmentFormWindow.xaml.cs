using HumanRecourcesApp.ViewModels;
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
    /// Interaction logic for DepartmentFormWindow.xaml
    /// </summary>
    public partial class DepartmentFormWindow : Window
    {
        public DepartmentFormWindow(User user)
        {
            InitializeComponent();
            DataContext = new DepartmentFormViewModel(user);
        }

        // Constructor with department for edit mode (convenience constructor)
        public DepartmentFormWindow(User user, Department department)
        {
            InitializeComponent();
            DataContext = new DepartmentFormViewModel(user, department); 
        }
    }
}
