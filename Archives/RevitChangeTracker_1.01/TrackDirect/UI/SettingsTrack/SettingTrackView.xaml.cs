using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using TrackDirect.Utilities;
using static TrackDirect.UI.AutoTrackDataStorageUtil;

namespace TrackDirect.UI
{
   
    /// <summary>
    /// Interaction logic for SettingTrackView.xaml
    /// </summary>
    public partial class SettingTrackView : Window
    {
       
        public SettingTrackView()
        {
            InitializeComponent();
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

    }
   
}

