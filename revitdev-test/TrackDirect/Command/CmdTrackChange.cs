using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using TrackDirect.Properties;
using TrackDirect.Utilities;
using System.Windows.Forms;
using System;
using TrackDirect.Models;
using TrackDirect.UI;
using System.Linq;

namespace TrackDirect
{


    [Transaction(TransactionMode.Manual)]
    public class CmdTrackChange : IExternalCommand
    {

        /// <summary>
        /// Current snapshot of database state.
        /// You could also store the entire element state 
        /// strings here, not just their hash code, to
        /// report their complete original and modified 
        /// values.
        /// </summary>
        public static string UserRevit { get; set; } = string.Empty;

        #region External Command Mainline Execute Method
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            return Execute(commandData.Application);
        }

        //Call this methos with the external command
        public Result Execute(UIApplication uiapp)
        {
            UserRevit = uiapp.Application.Username;
            //Start addin comparison
            try
            {
                AppCommand.TrackHandler.Request.Make(TrackDirectHandler.RequestId.TrackChangesCommand);
                AppCommand.ExEvent.Raise();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error! " + ex);
                return Result.Failed;
            }

        }
        #endregion // External Command Mainline Execute Method

    }
}
