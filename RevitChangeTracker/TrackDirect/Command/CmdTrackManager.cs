using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Windows.Forms;
using TrackDirect.UI;

namespace TrackDirect
{
    [Transaction(TransactionMode.Manual)]
    public class CmdTrackManager: IExternalCommand
    {
        public static UIApplication Uiapp = null;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Uiapp = commandData.Application;
            try
            {
                
                var vm = new TrackManagerViewModel(Uiapp);
                TrackManagerWindow wd = new TrackManagerWindow();
                wd.DataContext = vm;
                if (vm.DisplayUI())
                {
                    wd.Show();
                }
               

                return Result.Succeeded;
            }
            catch (ApplicationException aex)
            {
                MessageBox.Show(aex.Message);
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error! " + ex);
                return Result.Failed;
            }
        }
    }
}
