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
using System.Runtime.InteropServices;
using Autodesk.Windows;
using System.Threading;

namespace TrackDirect
{


    [Transaction(TransactionMode.Manual)]
    public class CmdAutoTrack : IExternalCommand
    {

        private static Thread _thread = null;

        private static int _timeOutMinutes = 1;
        private static int _timeout = 1000 * 60 * _timeOutMinutes;
        public static string UserRevit { get; set; } = string.Empty;
        private static bool isRunning = false;

        
        private Document _activeDoc;
        private Autodesk.Revit.ApplicationServices.Application _app;


        public static bool IsRunning { get { return isRunning; } set { isRunning = value; } }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            return Execute(commandData.Application);
        }

        //Call this methos with the external command
        public Result Execute(UIApplication uiapp)
        {
            UserRevit = uiapp.Application.Username;
            _activeDoc = uiapp.ActiveUIDocument.Document;
            _app = uiapp.Application;

            isRunning = !isRunning;

            //Check and create shared parameter if they do not exist
            CreateSharedParameter();
            //Start addin comparison
            try
            {
                if (isRunning)
                {
                    AppCommand.btnTrack.ItemText = "Recording\nin process";
                    AppCommand.btnTrack.ToolTip = "Add-in is running. Click here to stop it.";
                    AppCommand.btnTrack.LongDescription = $"Project is runninng: ";
                    AppCommand.btnTrack.LargeImage = ImageUtils.ConvertFromBitmap(Resources.toggle_on_32);
                    AppCommand.btnTrack.Image = ImageUtils.ConvertFromBitmap(Resources.toggle_on_16);

                    //If run, we will loop record by timer
                    OnRecordingDataChanges();
                }
                else
                {
                    AppCommand.btnTrack.ItemText = "Start\nTrack";
                    AppCommand.btnTrack.ToolTip = "Track the data changes in the model. Click here to start record data changes in model.";
                    AppCommand.btnTrack.LongDescription = "This command toogles between start or stop record modification tracking";
                    AppCommand.btnTrack.LargeImage = ImageUtils.ConvertFromBitmap(Resources.toggle_off_32);

                    //If click to stop record, we need a snapshot to compare the last modifications
                    AppCommand.TrackHandler.Request.Make(TrackDirectHandler.RequestId.TrackChangesCommand);
                    AppCommand.ExEvent.Raise();
                    AppCommand.SetFocusToRevit();
                }
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error! " + ex);
                return Result.Failed;
            }

        }

        private void OnRecordingDataChanges()
        {
            // Start a thread to raise it regularly.
            _thread = new Thread(TriggerTrackDirectHandler);
            _thread.Start();
        }
        /// <summary>
        /// Trigger a modification tracker snapshot at 
        /// regular intervals. Relinquish control and wait 
        /// for the specified timeout period between each 
        /// snapshot. This method runs in a separate thread.
        /// </summary>
        private static void TriggerTrackDirectHandler()
        {
            while (isRunning)
            {
                AppCommand.TrackHandler.Request.Make(TrackDirectHandler.RequestId.TrackChangesCommand);
                AppCommand.ExEvent.Raise();
                AppCommand.SetFocusToRevit();
                // Wait and relinquish control 
                // before next snapshot.
                Thread.Sleep(_timeout);
            }
        }

        private void CreateSharedParameter()
        {
            Document doc = _activeDoc;
            using (Transaction tx = new Transaction(doc))
            {
                //Check and add shared parameter to project
                try
                {
                    tx.Start("Add shared parameter");
                    //Create a list of category
                    CategorySet categories = CategoryUtils.GetModelCategories(doc, _app);
                    List<string> catList = new List<string>();
                    foreach (Category c in categories)
                    {
                        catList.Add(c.Name);
                    }
                    catList.Sort();
                    //Create Shared parameters if necessary
                    ParameterUtils.AddSharedParameters(_app, doc, categories);
                    tx.Commit();
                }

                catch (Exception ex)
                {
                    MessageBox.Show("Error! " + ex);
                    if (tx.HasStarted() == true)
                    {
                        tx.RollBack();
                    }
                }
            }
        }

    }
}
