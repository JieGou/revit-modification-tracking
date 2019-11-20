using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackChanges.DMU
{
    public class FamilyUpdater : IUpdater
    {
        static AddInId m_appId;
        static UpdaterId m_updaterId;
        Element _element = null;
        // constructor takes the AddInId for the add-in associated with this updater
        public FamilyUpdater(AddInId id)
        {
            m_appId = id;
            m_updaterId = new UpdaterId(m_appId, new Guid("646209C2-7AB4-4C27-9B8B-7F5E679653E8")); //Guid of addin
        }
        public void Execute(UpdaterData data)
        {
            Document doc = data.GetDocument();
            string pCreatedDate = "CreatedDate";
            string pModifiedDate = "ModifiedDate";
            string date = DateTime.Now.ToShortDateString();

            foreach (ElementId elemId in data.GetAddedElementIds())
            {
                Element e = doc.GetElement(elemId);
                if (e != null)
                    using (Transaction trans = new Transaction(doc))
                    {
                        trans.Start();
                        WriteOnParam(e, pCreatedDate, date, true);
                        trans.Commit();
                    }
            }
            foreach (ElementId elemId in data.GetModifiedElementIds())
            {
                Element e = doc.GetElement(elemId);
                if(e != null )
                    using (Transaction trans = new Transaction(doc))
                    {
                        trans.Start();
                        WriteOnParam(e, pModifiedDate, date, true);
                        trans.Commit();
                    }
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
            return m_updaterId;
        }

        public string GetUpdaterName()
        {
            return "Family updater";
        }
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
