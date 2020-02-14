using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using TrackChanges;

namespace TrackChanges.DMU
{
    [Transaction(TransactionMode.ReadOnly)]
    public class CmdFamilyUpdater : IExternalCommand
    {
        private FamilyUpdater _updater = null;
        public static bool IsActived;
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Application app = commandData.Application.Application;
            Document doc = uidoc.Document;
                try
                {
                    UpdaterCommand(app, doc);
                    return Result.Succeeded;
            
                }
                catch (Exception e)
                {
                    message = e.Message;
                    return Autodesk.Revit.UI.Result.Failed;
                }
        }
        public void UpdaterCommand(Application app, Document doc)
        {
                if (!IsActived)
                {
                    EnableUpdater(app,doc);
                }
                else
                {
                    DisableUpdater(app, doc);
                }
 
        }
        private void EnableUpdater(Application app, Document doc)
        {
            //Rename button --> On when start and off when stop
            IsActived = true;
            App.btnDMU.ItemText = "On DMU";
            App.btnDMU.LargeImage = ImageUtils.ConvertFromBitmap(TrackChanges.Properties.Resources.ToggleOnLarge);
            TaskDialog.Show("Dynamic Model Update", "Start track now...");


            _updater = new FamilyUpdater(app.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(_updater);

            //Create a filter for elements interest
            IList<ElementId> eleIds = new FilteredElementCollector(doc).WhereElementIsNotElementType().WhereElementIsViewIndependent().ToElementIds().ToList();
            CategorySet categories = CategoryUtils.GetCategoriesHasElements(doc, app);
            IList<ElementFilter> catFilter = new List<ElementFilter>();

            foreach (Category cat in categories)
            {
                catFilter.Add(new ElementCategoryFilter(cat.Id));
            }
            
            //Filter for all elements of all categories in model
            LogicalOrFilter filter = new LogicalOrFilter(catFilter);


            //Registry the update
            UpdaterRegistry.AddTrigger(_updater.GetUpdaterId(), filter, Element.GetChangeTypeGeometry());
            UpdaterRegistry.AddTrigger(_updater.GetUpdaterId(), filter, Element.GetChangeTypeElementAddition());

            //For type parameter: we must liste all parameter interest to add trigger -->todo

        }
       private void DisableUpdater(Application app, Document doc)
        {
            IsActived = false;
            App.btnDMU.ItemText = "Off DMU";
            App.btnDMU.LargeImage = ImageUtils.ConvertFromBitmap(TrackChanges.Properties.Resources.ToggleOfLarge);
            UpdaterRegistry.UnregisterUpdater(new FamilyUpdater(app.ActiveAddInId).GetUpdaterId());
            _updater = null;
            TaskDialog.Show("Dynamic Model Update", "Stop track");
        }
    }
}
