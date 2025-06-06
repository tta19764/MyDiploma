﻿using HumanRecourcesApp.ViewModels;
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
    /// Interaction logic for PayrollDetailsWindow.xaml
    /// </summary>
    public partial class PayrollDetailsWindow : Window
    {
        public PayrollDetailsWindow(User user, int payPeriodId, int emplouyeeId)
        {
            InitializeComponent();
            DataContext = new PayrollDetailsWindowViewModel(user, payPeriodId, emplouyeeId);
        }
    }
}
