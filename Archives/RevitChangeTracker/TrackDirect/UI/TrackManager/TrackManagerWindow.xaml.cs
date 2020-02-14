using Autodesk.Revit.UI;
using System;
using System.Windows;
using System.Windows.Interop;

namespace TrackDirect.UI
{
    /// <summary>
    /// Interaction logic for SuiviManagerView.xaml
    /// </summary>
    public partial class TrackManagerWindow : Window
    {
        public static TrackManagerWindow Instance { get; set; } = null;
        public TrackManagerWindow(TrackManagerViewModel vm)
        {
            if (!TrackManagerViewModel.IsOpen) //Enforce single window
            {
                InitializeComponent();
                Instance = this;
                var uiapp = TrackManagerViewModel.Uiapp;
                IntPtr revitWindow;

#if REVIT2018
                revitWindow = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle; // 2018
#else
                revitWindow = uiapp.MainWindowHandle; //Revit 2019 and above
#endif

                //Get window of Revit form Revit handle
                HwndSource hwndSource = HwndSource.FromHwnd(revitWindow);
                var windowRevitOpen = hwndSource.RootVisual as Window;

                this.Owner = windowRevitOpen; //Open when click Revit
                this.DataContext = vm;

                if (vm.DisplayUI())
                {
                    this.Show();
                }

            }
            if (Instance?.WindowState == WindowState.Minimized)
                Instance.WindowState = WindowState.Normal;

        }


    }
}
