#region References
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using TrackDirect.Models;
using TrackDirect.Utilities;
using System.Windows.Forms;
using TrackDirect.Properties;

#endregion

namespace TrackDirect.UI
{
    public class TrackDirectHandler : IExternalEventHandler
    {
        private static Document _docRun;
        private static string _docId;
        public Document ActiveDoc { get; set; }
        private UIApplication _uiapp;
        private Autodesk.Revit.ApplicationServices.Application _app;
        public static Dictionary<string, MiniComparisionContainer> _startState { get; private set; } = null;

        public string GetName()
        {
            return "Task External Event";
        }

        public void Execute(UIApplication uiapp)
        {
            _uiapp = uiapp;
            _app = uiapp.Application;
            ActiveDoc = uiapp.ActiveUIDocument.Document;
            TrackChangesCommand();
        }

        #region External event
        private void CreateSharedParameter()
        {
            Document doc = ActiveDoc;
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
        public void TrackChangesCommand()
        {
            CreateSharedParameter();
            Document activeDoc = _uiapp.ActiveUIDocument.Document;
            if (_startState is null)
            {
                //If command not running we will run it
                _docRun = activeDoc;
                _docId = _docRun.ProjectInformation.UniqueId;
                //Retrieve the coresponding list of elements
                var temp = CategoryDataStorageUtil.GetCategoryPropertiesDataStorage(_docRun);
                IList<ElementId> cats = temp.Where(x => x.Selected == true).Select(x => x.CategoryId).ToList();
                IEnumerable<Element> elems = ElementUtils.GetElementsByCategories(_docRun, cats);
                var com = GetComparision(_docRun, elems);
                _startState = com.TrackedData;
            }
            else
            {
                string docId2 = activeDoc.ProjectInformation.UniqueId;
                if (_docId != docId2)
                {
                    TaskDialog.Show("Change Document", "Document running is changed!");
                }
                //When document change, we need get data comparision of the previous document to compare
                //This will ensure that we always compare the same project
                var temp = CategoryDataStorageUtil.GetCategoryPropertiesDataStorage(_docRun);
                IList<ElementId> cats = temp.Where(x => x.Selected == true).Select(x => x.CategoryId).ToList();
                IEnumerable<Element> elems = ElementUtils.GetElementsByCategories(_docRun, cats);
                var com = GetComparision(_docRun, elems);
                Dictionary<string, MiniComparisionContainer> end_state = com.TrackedData;

                //Compare 2 data
                MiniComparisonController.MiniReportDifferences(activeDoc, _startState, end_state);

                _docRun = activeDoc;
                _docId = _docRun.ProjectInformation.UniqueId;
                _startState = null;
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

        #endregion //External events

    }
}