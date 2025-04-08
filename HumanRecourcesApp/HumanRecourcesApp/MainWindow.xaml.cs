using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HumanResourcesApp.Models;
using HumanResourcesApp.DBClasses;
using HumanRecourcesApp.ViewModels;

namespace HumanResourcesApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HumanResourcesDB _db;
        private readonly User _currentUser;

        public MainWindow(User user)
        {
            InitializeComponent();

            if (user == null) throw new ArgumentNullException(nameof(user), "User cannot be null");

            _db = new HumanResourcesDB();
            _currentUser = user;

            DataContext = new MainWindowViewModel(user);
            UpdateUserLastLogin(user.UserId);

        }

        private void UpdateUserLastLogin(int userId)
        {
            try
            {
                _db.UpdateLastLogin(userId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update last login: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}