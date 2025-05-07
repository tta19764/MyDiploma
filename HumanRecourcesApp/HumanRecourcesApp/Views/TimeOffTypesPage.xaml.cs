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
    /// Interaction logic for TimeOffTypesPage.xaml
    /// </summary>
    public partial class TimeOffTypesPage : Page
    {
        public TimeOffTypesPage(User user)
        {
            InitializeComponent();
            DataContext = new ViewModels.TimeOffTypesViewModel(user);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            // Allow only digits
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Allow navigation keys and delete/backspace
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
    }
}
