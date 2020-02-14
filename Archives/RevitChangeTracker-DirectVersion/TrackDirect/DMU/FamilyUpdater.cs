using Autodesk.Revit.Attributes;
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
    [Transaction(TransactionMode.ReadOnly)]
    public class FamilyUpdater : IUpdater
    {
        static AddInId _appId;
        static UpdaterId _updaterId;
        private bool _isActived;


        Element _element = null;

        // constructor takes the AddInId for the add-in associated with this updater
        public FamilyUpdater(AddInId id)
        {
            _isActived = CmdFamilyUpdater.IsActived;
            _appId = id;
            _updaterId = new UpdaterId(_appId, new Guid("646209C2-7AB4-4C27-9B8B-7F5E679653E8")); //Guid of updater
        }
        public void Execute(UpdaterData data)
        {
            //data is elements we want to get
            Document doc = data.GetDocument();
            string pCreatedDate = "CreatedDate";
            string pModifiedDate = "ModifiedDate";
            string date = DateTime.Now.ToShortDateString();

                try
                {
                    //Get list element instace (not element type in project)
                    IList<ElementId> allElemIds = ElementUtils.GetElementInProject(doc, false).Select(e => e.Id).ToList();
                    Dictionary<ElementId, ElementId> dicEleIds = allElemIds.ToDictionary<ElementId, ElementId>(id => id);


                    foreach (ElementId elemId in data.GetAddedElementIds())
                    {
                        //Avoid element type
                        if (dicEleIds.ContainsKey(elemId))
                        {
                        Element e = doc.GetElement(elemId);
                        if (e != null)
                                WriteOnParam(e, pCreatedDate, date, true);
                        }
                    }
                    foreach (ElementId elemId in data.GetModifiedElementIds())
                    {
                        if (dicEleIds.ContainsKey(elemId))
                        {
                        Element e = doc.GetElement(elemId);
                        if (e != null)
                                WriteOnParam(e, pModifiedDate, date, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                        throw;
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
