using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace HumanResourcesApp.Controls
{
    public partial class SectionHeader : UserControl
    {
        // Constructor
        public SectionHeader()
        {
            InitializeComponent();

            // Register for loaded event to check permissions after the control has been fully initialized
            this.Loaded += SectionHeader_Loaded;
        }

        // Text Property
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SectionHeader), new PropertyMetadata(string.Empty));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Required Permissions List Property
        public static readonly DependencyProperty PermissionsProperty =
            DependencyProperty.Register("Permissions", typeof(List<string>), typeof(SectionHeader),
                new PropertyMetadata(null, OnPermissionsChanged));

        public List<string> Permissions
        {
            get { return (List<string>)GetValue(PermissionsProperty); }
            set { SetValue(PermissionsProperty, value); }
        }

        private static void OnPermissionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SectionHeader control)
            {
                control.CheckPermissions();
            }
        }

        // PermissionSource Property
        public static readonly DependencyProperty PermissionSourceProperty =
            DependencyProperty.Register("PermissionSource", typeof(IPermissionContext), typeof(SectionHeader),
                new PropertyMetadata(null, OnPermissionSourceChanged));

        public IPermissionContext PermissionSource
        {
            get { return (IPermissionContext)GetValue(PermissionSourceProperty); }
            set { SetValue(PermissionSourceProperty, value); }
        }

        private static void OnPermissionSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SectionHeader control)
            {
                control.CheckPermissions();
            }
        }

        // Check if at least one permission in the list is granted
        private void CheckPermissions()
        {
            // If no permissions are specified, always show the header
            if (Permissions == null || Permissions.Count == 0)
            {
                this.Visibility = Visibility.Visible;
                return;
            }

            // If no permission source is provided, hide the header
            if (PermissionSource == null)
            {
                this.Visibility = Visibility.Collapsed;
                return;
            }

            // Check if user has access to at least one permission
            bool hasAnyAccess = false;
            foreach (var permission in Permissions)
            {
                // Empty permission means no specific permission required, so count as true
                if (string.IsNullOrEmpty(permission) || PermissionSource.UserHasAccess(permission))
                {
                    hasAnyAccess = true;
                    break;
                }
            }

            this.Visibility = hasAnyAccess ? Visibility.Visible : Visibility.Collapsed;
        }

        // Update permissions when loaded
        private void SectionHeader_Loaded(object sender, RoutedEventArgs e)
        {
            CheckPermissions();
        }
    }
}