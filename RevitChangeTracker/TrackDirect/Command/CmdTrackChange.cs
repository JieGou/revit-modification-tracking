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
        private static int _timeOutMinutes = 1;
        private static int _timeout = 1000 * 60 * _timeOutMinutes;
        public static string UserRevit { get; set; } = string.Empty;

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
                AppCommand.SetFocusToRevit();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error! " + ex);
                return Result.Failed;
            }

        }
    }
}
