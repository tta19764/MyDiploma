using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HumanResourcesApp.Controls
{
    public partial class NavButton : UserControl
    {
        // Constructor
        public NavButton()
        {
            InitializeComponent();
            UpdateButtonStyle();

            // Register for loaded event to check permissions after the control has been fully initialized
            this.Loaded += UserControl_Loaded;
        }

        // Text Property
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(NavButton), new PropertyMetadata(string.Empty));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // IsActive Property
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(NavButton),
                new PropertyMetadata(false, OnIsActiveChanged));

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NavButton control)
            {
                control.UpdateButtonStyle();
            }
        }

        // Command Property
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(NavButton), new PropertyMetadata(null));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // CommandParameter Property
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(NavButton), new PropertyMetadata(null));

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // RequiredPermission Property
        public static readonly DependencyProperty RequiredPermissionProperty =
            DependencyProperty.Register("RequiredPermission", typeof(string), typeof(NavButton),
                new PropertyMetadata(string.Empty, OnRequiredPermissionChanged));

        public string RequiredPermission
        {
            get { return (string)GetValue(RequiredPermissionProperty); }
            set { SetValue(RequiredPermissionProperty, value); }
        }

        private static void OnRequiredPermissionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NavButton control)
            {
                control.CheckPermissions();
            }
        }

        // PermissionSource Property
        public static readonly DependencyProperty PermissionSourceProperty =
            DependencyProperty.Register("PermissionSource", typeof(IPermissionContext), typeof(NavButton),
                new PropertyMetadata(null, OnPermissionSourceChanged));

        public IPermissionContext PermissionSource
        {
            get { return (IPermissionContext)GetValue(PermissionSourceProperty); }
            set { SetValue(PermissionSourceProperty, value); }
        }

        private static void OnPermissionSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NavButton control)
            {
                control.CheckPermissions();
            }
        }

        // Updates the button styling based on active state
        private void UpdateButtonStyle()
        {
            if (MainButton != null)
            {
                if (IsActive)
                {
                    MainButton.Background = new SolidColorBrush(Color.FromRgb(44, 62, 80)); // #2c3e50
                    MainButton.Foreground = new SolidColorBrush(Colors.White);
                    MainButton.FontWeight = FontWeights.SemiBold;
                }
                else
                {
                    MainButton.Background = new SolidColorBrush(Colors.Transparent);
                    MainButton.Foreground = new SolidColorBrush(Color.FromRgb(221, 221, 221)); // #DDDDDD
                    MainButton.FontWeight = FontWeights.Normal;
                }
            }
        }

        // Check if the user has the required permission
        private void CheckPermissions()
        {
            // If no permission is required, always show the button
            if (string.IsNullOrEmpty(RequiredPermission))
            {
                this.Visibility = Visibility.Visible;
                return;
            }

            // If no permission source is provided, hide the button
            if (PermissionSource == null)
            {
                this.Visibility = Visibility.Collapsed;
                return;
            }

            // Check if user has access
            bool hasAccess = PermissionSource.UserHasAccess(RequiredPermission);
            this.Visibility = hasAccess ? Visibility.Visible : Visibility.Collapsed;
        }

        // Update permissions when loaded
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CheckPermissions();
        }
    }
}