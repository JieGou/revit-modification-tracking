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
    public class CmdTestTreeView: IExternalCommand
    {
        public static Window1 _window1 { get; set; }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                Window1 wd = new Window1();
                _window1 = wd;
                wd.DataContext = new FooViewModel();

                wd.ShowDialog();

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
