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
        private static Document _activeDoc;
        private static UIApplication _uiapp;
        private static Autodesk.Revit.ApplicationServices.Application _app;
        public static Dictionary<string, MiniDataComparision> _startState { get; private set; } = null;

        public string GetName()
        {
            return "Task External Event";
        }

        public void Execute(UIApplication uiapp)
        {
            _uiapp = uiapp;
            _app = uiapp.Application;
            _activeDoc = uiapp.ActiveUIDocument.Document;
            try
            {
                switch (Request.Take())
                {
                    case RequestId.None:
                        {
                            return;
                        }
                    case RequestId.TrackChangesCommand:
                        {
                            GetSnapShot();
                            break;
                        }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public CommunicatorRequest Request { get; set; } = new CommunicatorRequest();
        public class CommunicatorRequest
        {
            private int _request = (int)RequestId.None;

            public RequestId Take()
            {
                return (RequestId)Interlocked.Exchange(ref _request, (int)RequestId.None);
            }

            public void Make(RequestId request)
            {
                Interlocked.Exchange(ref _request, (int)request);
            }
        }
        public enum RequestId
        {
            None,
            TrackChangesCommand
        }

        #region External event

        public void GetSnapShot()
        {
            Document activeDoc = _uiapp.ActiveUIDocument.Document;
            _docRun = _uiapp.ActiveUIDocument.Document;
            _docId = _docRun.ProjectInformation.UniqueId;
            //MessageBox.Show("Start Run executed!", "Multi-Threading");
            //check and create shared parameters
            CreateSharedParameter();


            //First time run
            if (MiniComparisonContainer.ProjectId == "")
            {
                //Save project id
                MiniComparisonContainer.Doc = _docRun;
                MiniComparisonContainer.ProjectId = _docId;
                AppCommand.btnTrack.LongDescription = $"Project is runninng: {_docRun.Title}";

                //Retrieve the coresponding list of elements
                var elems = GetTrackedElement(_docRun);
                GetCurrentDataComparision(_docRun, elems);
            }
            else
            {
                if (MiniComparisonContainer.ProjectId == _docId)
                {
                    MiniComparisonContainer.Doc = _docRun; //To Avoid Doc null
                    AppCommand.btnTrack.LongDescription = $"Project is runninng: {_docRun.Title}";
                    MiniComparisonContainer.PreviousData.Clear();
                    MiniComparisonContainer.PreviousData = MiniComparisonContainer.CurrentData.ToDictionary(entry => entry.Key, entry => entry.Value);
                    var elems = GetTrackedElement(_docRun);
                    GetCurrentDataComparision(_docRun, elems);
                    MiniComparisonController.MiniReportDifferences(_docRun, MiniComparisonContainer.PreviousData, MiniComparisonContainer.CurrentData);
                }
                else //ProjectId != _docId --> Document tracked is changed
                {
                    //If Project changed, we need get data from previous doc to compare
                    MiniComparisonContainer.PreviousData.Clear();
                    MiniComparisonContainer.PreviousData = MiniComparisonContainer.CurrentData.ToDictionary(entry => entry.Key, entry => entry.Value);

                    //Avoid error if MiniCoparisonContainer.Doc null
                    if(MiniComparisonContainer.Doc != null)
                    {
                        var elems1 = GetTrackedElement(MiniComparisonContainer.Doc);
                        GetCurrentDataComparision(MiniComparisonContainer.Doc, elems1);
                        MiniComparisonController.MiniReportDifferences(MiniComparisonContainer.Doc, MiniComparisonContainer.PreviousData, MiniComparisonContainer.CurrentData);
                    }
                    else
                    {
                        //Get document from guid
                        
                    }
                    
                    //***********************************************//
                    //Reset Data comparison for new document
                    //We consider this cas is first time running in new document (as case 1)

                    MiniComparisonContainer.Doc = _docRun;
                    MiniComparisonContainer.ProjectId = _docId;
                    MiniComparisonContainer.PreviousData.Clear();
                    var elems = GetTrackedElement(_docRun);
                    GetCurrentDataComparision(_docRun, elems);
                    AppCommand.btnTrack.LongDescription = $"Project is runninng: {_docRun}";
                }
            }

            //After run compare, we need check if user turnoff CmdTrackChange
            if(!CmdAutoTrack.IsRunning)
            {
                MiniComparisonContainer.ProjectId = "";
                MiniComparisonContainer.Doc = null;
                MiniComparisonContainer.PreviousData.Clear();
                MiniComparisonContainer.CurrentData.Clear();
            }
            //MessageBox.Show("Stop Run executed!", "Multi-Threading");
        }

        private void GetCurrentDataComparision(Document doc, IEnumerable<Element> elems)
        {
            MiniComparisonContainer.CurrentData.Clear();
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
                MiniComparisonContainer.CurrentData.Add(e.UniqueId, miniContainer);
            }
        }
       
        private IEnumerable<Element> GetTrackedElement(Document doc)
        {
            var tmp = CategoryDataStorageUtil.GetCategoryPropertiesDataStorage(doc);
            IList<ElementId> cats1 = tmp.Where(x => x.Selected == true).Select(x => x.CategoryId).ToList();
            return ElementUtils.GetElementsByCategories(doc, cats1);
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
        #endregion //External events

    }
}