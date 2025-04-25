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
        }

        #region DependencyProperties

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
            if (d is NavButton control) // Use pattern matching to avoid null check and cast explicitly  
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

        #endregion

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
    }
}