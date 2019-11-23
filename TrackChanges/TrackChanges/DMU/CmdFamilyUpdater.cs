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
        private static bool _isActived;
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Application app = commandData.Application.Application;
            Document doc = uidoc.Document;
            using (Transaction tx = new Transaction(doc))
            {
                try
                {
                    UpdaterCommand(app, doc);

                }
                catch (ErrorMessageException errorEx)
                {
                    // checked exception need to show in error messagebox
                    message = errorEx.Message;
                    if (tx.HasStarted() == true)
                    {
                        tx.RollBack();
                    }
                    return Autodesk.Revit.UI.Result.Failed;
                }
                catch (Exception ex)
                {
                    // unchecked exception cause command failed
                    message = ex.Message;
                    //Trace.WriteLine(ex.ToString());
                    if (tx.HasStarted() == true)
                    {
                        tx.RollBack();
                    }
                    return Autodesk.Revit.UI.Result.Failed;
                }
            }
                           
            return Result.Succeeded;
            
        }
        public void UpdaterCommand(Application app, Document doc)
        {
                if (!_isActived)
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
            _isActived = true;
            App.btnDMU.ItemText = "On DMU";
            (App.btnDMU as PushButton).LargeImage = ImageUtils.ConvertFromBitmap(TrackChanges.Properties.Resources.ToggleOnLarge);
            //App.btnDataTrackChange.LargeImage = ImageUtils.ConvertFromBitmap(TrackChanges.Properties.Resources.ToggleOnLarge);
            TaskDialog.Show("Dynamic Model Update", "Start check the modification of model now...");


            _updater = new FamilyUpdater(app.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(_updater);

            //Create a filter for elements interest
            CategorySet categories = CategoryUtils.GetCategoriesHasElements(doc, app);
            IList<ElementFilter> categoryFilters = new List<ElementFilter>();

            foreach (Category category in categories)
            {
                categoryFilters.Add(new ElementCategoryFilter(category.Id));
            }


            //Filter for all elements of all categories in model
            ElementClassFilter fiFilter= new ElementClassFilter(typeof(FamilyInstance));
            LogicalOrFilter catFilter = new LogicalOrFilter(categoryFilters);
            LogicalAndFilter filter = new LogicalAndFilter(fiFilter, catFilter);

            //Registry the update
            UpdaterRegistry.AddTrigger(_updater.GetUpdaterId(), filter, Element.GetChangeTypeGeometry());
            UpdaterRegistry.AddTrigger(_updater.GetUpdaterId(), filter, Element.GetChangeTypeElementAddition());

            //For type parameter: we must liste all parameter interest to add trigger

        }
       private void DisableUpdater(Application app, Document doc)
        {
            App.btnDMU.ItemText = "Off DMU";
            (App.btnDMU as PushButton).LargeImage = ImageUtils.ConvertFromBitmap(TrackChanges.Properties.Resources.ToggleOfLarge);
            //App.btnDataTrackChange.LargeImage = ImageUtils.ConvertFromBitmap(TrackChanges.Properties.Resources.ToggleOfLarge);// n'as pas marcher
            UpdaterRegistry.UnregisterUpdater(_updater.GetUpdaterId());
            _updater = null;
            _isActived = false;
            TaskDialog.Show("Dynamic Model Update", "Stop track");
        }
    }
}
