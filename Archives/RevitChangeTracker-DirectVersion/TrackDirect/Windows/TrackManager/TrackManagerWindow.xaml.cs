﻿using Autodesk.Revit.UI;
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

namespace TrackDirect.UI
{
    /// <summary>
    /// Interaction logic for SuiviManagerView.xaml
    /// </summary>
    public partial class TrackManagerWindow : Window
    {
        public TrackManagerWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ColorSelectionDialog colorSelectionDialog = new ColorSelectionDialog();
            colorSelectionDialog.Show();
        }
    }
}
