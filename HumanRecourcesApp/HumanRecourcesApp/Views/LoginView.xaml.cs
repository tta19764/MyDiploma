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
using System.Windows.Shapes;

namespace HumanResourcesApp.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            string password = passwordBox?.Password ?? string.Empty;

            // Get the current instance of LoginViewModel from DataContext
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.Password = PasswordBox.Password;

                if (string.IsNullOrWhiteSpace(password))
                {
                    viewModel.PasswordError = "Password is required";
                }
                else
                {
                    // Clear the error message if password is not empty
                    viewModel.PasswordError = string.Empty;
                }

                // Clear the general error message if it exists
                if (!string.IsNullOrEmpty(viewModel.ErrorMessage))
                {
                    viewModel.ErrorMessage = string.Empty;
                }
            }
        }
    }
}
