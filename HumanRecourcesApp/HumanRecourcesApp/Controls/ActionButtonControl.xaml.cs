using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HumanResourcesApp.Controls
{
    public partial class ActionButtonControl : UserControl
    {
        public ActionButtonControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ButtonCommandProperty =
            DependencyProperty.Register("ButtonCommand", typeof(ICommand), typeof(ActionButtonControl), new PropertyMetadata(null));

        public ICommand ButtonCommand
        {
            get => (ICommand)GetValue(ButtonCommandProperty);
            set => SetValue(ButtonCommandProperty, value);
        }

        public static readonly DependencyProperty ButtonContentProperty =
            DependencyProperty.Register("ButtonContent", typeof(object), typeof(ActionButtonControl), new PropertyMetadata("Button"));

        public object ButtonContent
        {
            get => GetValue(ButtonContentProperty);
            set => SetValue(ButtonContentProperty, value);
        }

        public static readonly DependencyProperty ButtonPaddingProperty =
    DependencyProperty.Register(
        nameof(ButtonPadding),
        typeof(Thickness),
        typeof(ActionButtonControl),
        new PropertyMetadata(new Thickness(12, 8, 12, 8)));

        public Thickness ButtonPadding
        {
            get => (Thickness)GetValue(ButtonPaddingProperty);
            set => SetValue(ButtonPaddingProperty, value);
        }


    }
}
