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
    public class CmdTrackDirect : IExternalCommand
    {

        /// <summary>
        /// Current snapshot of database state.
        /// You could also store the entire element state 
        /// strings here, not just their hash code, to
        /// report their complete original and modified 
        /// values.
        /// </summary>
        //public CmdTrackDirect() { }
        private Autodesk.Revit.ApplicationServices.Application _app = null;
        private static Document _docRun;
        private static string _docId;
        public static Dictionary<string, MiniComparisionContainer> _startState { get; private set; } = null;
        public static bool IsTracking { get; private set; } = false;
        public static string UserRevit { get; set; } = string.Empty;

        #region External Command Mainline Execute Method
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            return Execute(commandData.Application);
        }

        //Call this methos with the external command
        public Result Execute(UIApplication uiapp)
        {

            UIDocument uidoc = uiapp.ActiveUIDocument;
            _app = uiapp.Application;
            Document doc = uidoc.Document;
            UserRevit = _app.Username;
            //Add shared parameters
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
                    return Autodesk.Revit.UI.Result.Failed;
                }
            }

            //Start addin comparison
            try
            {
                TrackChangesCommand(doc);
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error! " + ex);
                return Result.Failed;
            }



        }
        #endregion // External Command Mainline Execute Method
        public void TrackChangesCommand(Document doc)
        {

            if (_startState is null)
            {
                //Change Icon and text of button
                AppCommand.btnTrack.ItemText = "Stop\nTrack";
                AppCommand.btnTrack.ToolTip = "Add-in is running. Clic here to stop it.";
                AppCommand.btnTrack.LongDescription = $"Project is runninng: {doc.Title}";
                AppCommand.btnTrack.LargeImage = ImageUtils.ConvertFromBitmap(Resources.toggle_on_32);
                AppCommand.btnTrack.Image = ImageUtils.ConvertFromBitmap(Resources.toggle_on_16);

                //If command not running we will run it
                _docRun = doc;
                _docId = _docRun.ProjectInformation.UniqueId;
                //Retrieve the coresponding list of elements
                var temp = CategoryDataStorageUtil.GetCategoryPropertiesDataStorage(doc);
                IList<ElementId> cats = temp.Where(x => x.Selected == true).Select(x => x.CategoryId).ToList();
                IEnumerable<Element> elems = ElementUtils.GetElementsByCategories(_docRun,cats);
                IsTracking = true;
                var com = GetComparision(_docRun, elems);
                _startState = com.TrackedData;
            }
            else
            {
                AppCommand.btnTrack.ItemText = "Run\nTrack";
                AppCommand.btnTrack.ToolTip = "Track the change in the model. Clic here to run this add-in.";
                AppCommand.btnTrack.LongDescription = "This command toogles between starting and ending modification tracking";
                AppCommand.btnTrack.LargeImage = ImageUtils.ConvertFromBitmap(Resources.toggle_off_32);
                string docId2 = doc.ProjectInformation.UniqueId;
                if (_docId != docId2)
                {
                    TaskDialog.Show("Change Document", "Document running is changed!");
                }
                //When document change, we need get data comparision of the previous document to compare
                //This will ensure that we always compare the same project
                var temp = CategoryDataStorageUtil.GetCategoryPropertiesDataStorage(doc);
                IList<ElementId> cats = temp.Where(x => x.Selected == true).Select(x => x.CategoryId).ToList();
                IEnumerable<Element> elems = ElementUtils.GetElementsByCategories(_docRun, cats);
                var com = GetComparision(_docRun, elems);
                Dictionary<string, MiniComparisionContainer> end_state = com.TrackedData;

                //Compare 2 data
                MiniComparisonController.MiniReportDifferences(doc, _startState, end_state);

                _docRun = doc;
                _docId = _docRun.ProjectInformation.UniqueId;
                _startState = null;
                IsTracking = false;
            }
        }
        private MiniComparison GetComparision(Document doc, IEnumerable<Element> elems)
        {
            MiniComparison com = new MiniComparison();
            com.projectId = doc.ProjectInformation.UniqueId;
            foreach (var e in elems)
            {
                Category c = e.Category;
                if (e is FamilySymbol)
                {
                    FamilySymbol fs = e as FamilySymbol;
                    c = fs.Family.FamilyCategory;
                }
                if (c == null) continue; // we don't want these things?
                MiniComparisonMaker miniMaker = new MiniComparisonMaker(doc);
                var miniContainer = miniMaker.GetHashSetDataComparision(e);
                com.projectId = doc.ProjectInformation.UniqueId;
                com.TrackedData.Add(e.UniqueId, miniContainer);
            }
            return com;
        }

    }
}
