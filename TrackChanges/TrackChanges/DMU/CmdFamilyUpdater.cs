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
            try
            {
                if (_updater is null)
                {
                    //Rename button --> On when start and off when stop
                    App.btnTrackChange.ItemText = "On DMU";
                    //App.btnDataTrackChange.LargeImage = ImageUtils.ConvertFromBitmap(TrackChanges.Properties.Resources.ToggleOnLarge);
                    TaskDialog.Show("Dynamic Model Update", "Start check the modification of model now...");


                    _updater = new FamilyUpdater(app.ActiveAddInId);
                    UpdaterRegistry.RegisterUpdater(_updater);

                    //Create a filter for elements interest
                    CategorySet categories = CategoryUtils.CreateCategoryList(doc, app);
                    IList<ElementFilter> categoryFilters = new List<ElementFilter>();

                    foreach (Category category in categories)
                    {
                        categoryFilters.Add(new ElementCategoryFilter(category.Id));
                    }
                    ElementFilter filter = new LogicalOrFilter(categoryFilters);

                    //Registry the update
                    UpdaterRegistry.AddTrigger(_updater.GetUpdaterId(), filter, Element.GetChangeTypeGeometry());
                    UpdaterRegistry.AddTrigger(_updater.GetUpdaterId(), filter, Element.GetChangeTypeElementAddition());
                }
                else
                {
                    App.btnTrackChange.ItemText = "Off DMU";
                    //App.btnDataTrackChange.LargeImage = ImageUtils.ConvertFromBitmap(TrackChanges.Properties.Resources.ToggleOfLarge);// n'as pas marcher
                    UpdaterRegistry.UnregisterUpdater(_updater.GetUpdaterId());
                    _updater = null;
                    TaskDialog.Show("Dynamic Model Update", "Stop track");
                }
            }
            catch (Exception)
            {

                throw;
            }        
            
        }
       
    }
}
