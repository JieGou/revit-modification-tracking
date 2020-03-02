using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackChanges.DMU
{
    public class WallUpdater :IUpdater
    {
        static AddInId m_appId;
        static UpdaterId m_updaterId;
        WallType m_wallType = null;
        // constructor takes the AddInId for the add-in associated with this updater
        public WallUpdater(AddInId id)
        {
            m_appId = id;
            m_updaterId = new UpdaterId(m_appId, new Guid("1FD73F30-B725-4375-8B5F-6CEFF23CE1CA")); //Guid of addin
        }
        public void Execute(UpdaterData data)
        {
            Document doc = data.GetDocument();
            // Cache the wall type
            if (m_wallType == null)
            {
                FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(WallType));

                var wallTypes = from element in collector
                                where
                                element.Name == "Wall concreate"
                                select element;
                if (wallTypes.Count<Element>() > 0)
                {
                    m_wallType = wallTypes.Cast<WallType>().ElementAt<WallType>(0);
                }
            }
            if (m_wallType != null)
            {
                // Change the wall to the cached wall type.
                foreach (ElementId addedElemId in data.GetAddedElementIds())
                {
                    Wall wall = doc.GetElement(addedElemId) as Wall;
                    if (wall != null)
                    {
                        wall.WallType = m_wallType;
                    }
                }
            }
        }
        public string GetAdditionalInformation()
        {
            return "Wall type updater example: updates all newly created walls to a special wall";
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
            return "Wall Type Updater";
        }
    }
    
}

