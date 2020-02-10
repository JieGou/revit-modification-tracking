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
                if (TrackDirectHandler._startState is null)
                {
                    //Change Icon and text of button
                    AppCommand.btnTrack.ItemText = "Stop\nTrack";
                    AppCommand.btnTrack.ToolTip = "Add-in is running. Clic here to stop it.";
                    AppCommand.btnTrack.LongDescription = $"Project is runninng: {uiapp.ActiveUIDocument.Document.Title}";
                    AppCommand.btnTrack.LargeImage = ImageUtils.ConvertFromBitmap(Resources.toggle_on_32);
                    AppCommand.btnTrack.Image = ImageUtils.ConvertFromBitmap(Resources.toggle_on_16);
                    AppCommand.TrackExEvent.Raise();
                }
                else
                {
                    AppCommand.btnTrack.ItemText = "Run\nTrack";
                    AppCommand.btnTrack.ToolTip = "Track the change in the model. Clic here to run this add-in.";
                    AppCommand.btnTrack.LongDescription = "Nothing";
                    AppCommand.btnTrack.LargeImage = ImageUtils.ConvertFromBitmap(Resources.toggle_off_32);
                    AppCommand.TrackExEvent.Raise();
                }
               
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
