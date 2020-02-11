using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using TrackChanges.Properties;

namespace TrackChanges
{
    

    [Transaction(TransactionMode.Manual)]
    public class CmdTrackChange :IExternalCommand
    {

        /// <summary>
        /// Current snapshot of database state.
        /// You could also store the entire element state 
        /// strings here, not just their hash code, to
        /// report their complete original and modified 
        /// values.
        /// </summary>
        public CmdTrackChange() { }
        static Dictionary<int, string> _start_state = null;

        #region External Command Mainline Execute Method
        public Result Execute(ExternalCommandData commandData,ref string message,ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            

            TrackChangesCommand(doc);
            return Result.Succeeded;
        }
        #endregion // External Command Mainline Execute Method
        public void TrackChangesCommand(Document doc)
        {
            IEnumerable<Element> a = ElementUtils.GetTrackedElements(doc);

            if (_start_state is null)
            {
                App.btnTrackChange.ItemText = "On Track";
                App.btnTrackChange.LargeImage = ImageUtils.ConvertFromBitmap(Resources.icon);
                _start_state = ReportChange.GetSnapshot(a);
                TaskDialog.Show("Track Changes",
                  "Started tracking changes now.");
            }
            else
            {
                App.btnTrackChange.ItemText = "Off Track";
                App.btnTrackChange.LargeImage = ImageUtils.ConvertFromBitmap(Resources.icon);
                Dictionary<int, string> end_state = ReportChange.GetSnapshot(a);
                ReportChange.ReportDifferences(doc, _start_state, end_state);
                _start_state = null;
            }
        }
      
    }
}
