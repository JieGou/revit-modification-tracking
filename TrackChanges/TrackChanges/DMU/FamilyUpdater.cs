using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackChanges.DMU
{
    public class FamilyUpdater : IUpdater
    {
        static AddInId _appId;
        static UpdaterId _updaterId;
        public static bool _isActived = false;
        

        Element _element = null;
        // constructor takes the AddInId for the add-in associated with this updater
        public FamilyUpdater(AddInId id)
        {
            _appId = id;
            _updaterId = new UpdaterId(_appId, new Guid("646209C2-7AB4-4C27-9B8B-7F5E679653E8")); //Guid of addin
            _isActived = true;

        }
        public void Execute(UpdaterData data)
        {
            Document doc = data.GetDocument();
            string pCreatedDate = "CreatedDate";
            string pModifiedDate = "ModifiedDate";
            string date = DateTime.Now.ToShortDateString();




            foreach (ElementId elemId in data.GetAddedElementIds())
            {
                //Element e = doc.GetElement(elemId);
                //if (e != null)
                //    using (Transaction trans = new Transaction(doc))
                //    {
                //        trans.Start();
                //        WriteOnParam(e, pCreatedDate, date, true);
                //        trans.Commit();
                //    }
                TaskDialog.Show("DMU", "Add element");
            }
            foreach (ElementId elemId in data.GetModifiedElementIds())
            {
                //Element e = doc.GetElement(elemId);
                //if(e != null )
                //    using (Transaction trans = new Transaction(doc))
                //    {
                //        trans.Start();
                //        WriteOnParam(e, pModifiedDate, date, true);
                //        trans.Commit();
                //    }
                TaskDialog.Show("DMU", "Modify element");
            }

        }

        public string GetAdditionalInformation()
        {
            return "Update all family instance";
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.FloorsRoofsStructuralWalls;
        }

        public UpdaterId GetUpdaterId()
        {
            return _updaterId;
        }

        public string GetUpdaterName()
        {
            return "Family updater";
        }

        //private void uiapp_Application_FailuresProcessing(object sender, FailuresProcessingEventArgs e)
        //{
        //    string transactionName = e.GetFailuresAccessor().GetTransactionName();
        //    isReloading = transactionName.Equals("Reload Latest");
        //    if (transactionName.Equals("Synchronize with Central")) isSynchronizing = true;
        //    if (transactionName.Equals("Stop sharing")) isSynchronizing = false;
        //}



        private void WriteOnParam(Element e, string paramName, string value, bool overrideValues = true)
        {
            IList<Parameter> parameters = e.GetParameters(paramName);
            if (parameters.Count != 0)
            {
                Parameter p = parameters.FirstOrDefault();
                if (!p.IsReadOnly)
                {
                    if (overrideValues)
                    {
                        p.Set(value);
                    }
                    else
                    {
                        string paramValue = p.AsString();
                        if (String.IsNullOrEmpty(paramValue))
                        {
                            p.Set(value);
                        }
                    }
                }
            }
        }

    }
}
