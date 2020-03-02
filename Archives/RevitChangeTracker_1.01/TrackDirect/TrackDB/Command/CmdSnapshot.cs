using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Windows.Forms;

namespace TrackDirect
{
    [Transaction(TransactionMode.Manual), Journaling(JournalingMode.UsingCommandData)]
    public class CmdSnapshot : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            try
            {

                AppCommand.FirstTimeRun(); // analytics
                Document doc = commandData.Application.ActiveUIDocument.Document;

                IList<Document> inMemory = Utilities.RevitUtils.GetProjectsInMemory(commandData.Application.Application);

                UI.frmSnapshot formSnap = new UI.frmSnapshot(inMemory, doc);
                formSnap.ShowDialog();
                if (!formSnap.DialogResult.HasValue || !formSnap.DialogResult.Value) return Result.Cancelled;
                string fileName = formSnap.Filename;

                //store for the future? seems like there are suddenly problems with reading this info back...
                commandData.JournalData.Add("DocumentName", formSnap.SelectedDocument.Title);
                commandData.JournalData.Add("Filename", formSnap.Filename);

                SnapshotMaker maker = new SnapshotMaker(formSnap.SelectedDocument, formSnap.Filename);
                maker.Export();


                TaskDialog td = new TaskDialog("Fingerprint");
                td.MainContent = "The snapshot file has been created.";
                td.ExpandedContent = "File: " + fileName + Environment.NewLine + "Duration: " + maker.Duration.TotalMinutes.ToString("F2") + " minutes.";
                td.Show();

                GC.Collect();
                GC.WaitForPendingFinalizers();

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

        /// <summary>
        /// Batch version, so that others can call it.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="filename"></param>
        public static void Export(Document doc, string filename)
        {
            //doc.Application.WriteJournalComment("Launching Batch Metamorphosis Snapshot...", false);
            //doc.Application.WriteJournalComment("  Filename: " + filename, false);
            //SnapshotMaker maker = new SnapshotMaker(doc, filename);
            //maker.Export();

            //doc.Application.WriteJournalComment("Snapshot completed. Duration:  " + maker.Duration, false);
            //doc.Application.WriteJournalComment("Garbage Collection to release db...", false);
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //doc.Application.WriteJournalComment("Garbage is collected.", false);

        }
    }
}
