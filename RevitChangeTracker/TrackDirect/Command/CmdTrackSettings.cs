using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Windows;
using System.Windows.Forms;
using TrackDirect.UI;
using TrackDirect.Utilities;
using static TrackDirect.UI.SettingTrackView;
using System.Windows.Interop;
using System.Windows;

namespace TrackDirect
{
    [Transaction(TransactionMode.Manual)]
    public class CmdTrackSettings : IExternalCommand
    {
        public static UIApplication Uiapp = null;
        public static IntPtr RevitWindow;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                //IWin32Window revit_window = new JtWindowHandle( ComponentManager.ApplicationWindow);
                Uiapp = commandData.Application;
                RevitWindow = Uiapp.MainWindowHandle; //Revit 2019

                //_revit_window
                //  = new JtWindowHandle(
                //    ComponentManager.ApplicationWindow ); // 2018

                //Get window of Revit form Revit handle
                HwndSource hwndSource = HwndSource.FromHwnd(RevitWindow);
                var WindowRevitOpen = hwndSource.RootVisual as Window;

                var vm = new SettingsTrackViewModel(Uiapp);
                SettingTrackView trackView = new SettingTrackView();
                trackView.DataContext = vm;
                if (vm.DisplayUI())
                {
                    trackView.Owner = WindowRevitOpen;//This will make flash this window WPF for modal mode
                    trackView.ShowDialog();
                }
                //Reset top button for split button
                AppCommand.GetInstance.SetTopButtonCurrent();
                return Result.Succeeded;
            }
            catch (ApplicationException aex)
            {
                System.Windows.Forms.MessageBox.Show(aex.Message);
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error! " + ex);
                return Result.Failed;
            }
        }


    }
}
