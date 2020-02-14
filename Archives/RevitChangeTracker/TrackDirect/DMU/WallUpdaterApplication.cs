using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using TrackChanges.Properties;
using System.Reflection;

namespace TrackChanges.DMU
{
    public class WallUpdaterApplication : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            // Register wall updater with Revit
            WallUpdater updater = new WallUpdater(application.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(updater);
            // Change Scope = any Wall element
            ElementClassFilter wallFilter = new ElementClassFilter(typeof(Wall));
            // Change type = element addition
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), wallFilter, Element.GetChangeTypeElementAddition());
            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication application)
        {
            WallUpdater updater = new WallUpdater(application.ActiveAddInId);
            UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
            return Result.Succeeded;
        }

       
    }
}
