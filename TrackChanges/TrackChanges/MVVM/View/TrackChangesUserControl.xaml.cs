﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Autodesk.Revit.UI;

namespace TrackChanges
{
    /// <summary>
    /// UI Events
    /// </summary>
    public partial class TrackChangesUserControl : UserControl
    {
        public static TrackChangesUserControl Instance;

        public ViewModel ViewModel;
        private ExternalEvent _event;

        public TrackChangesUserControl(ExternalEvent exEvent)
        {
            InitializeComponent();

            Instance = this;
            ViewModel = new ViewModel();
            DataContext = ViewModel;
            _event = exEvent;
            _event.Raise();
        }
    }
}
