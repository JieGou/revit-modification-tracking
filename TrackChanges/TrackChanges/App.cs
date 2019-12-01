#region Namespaces
using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using TrackChanges.Properties;
using System.Reflection;


#endregion

namespace TrackChanges
{
    class App : IExternalApplication
    {
        internal static string assemblyPath = typeof(App).Assembly.Location;
        internal static App GetInstance { get; }

        //Get button to change their properties in class command
        internal static PushButton btnDMU { get; set; }
        internal static PushButton btnTrackChange { get; set; }


        #region Result Startup
        public Result OnStartup(UIControlledApplication a)
        {
            CreateRibbonItem(a);
            //try
            //{
            //    // Register event. 
            //    //a.ControlledApplication.DocumentOpened += new EventHandler<DocumentOpenedEventArgs>(IUpdaterInitialized);
            //}
            //catch
            //{
            //    return Result.Failed;
            //}
            return Result.Succeeded;
        }
        #endregion

        #region Result Shutdown
        public Result OnShutdown(UIControlledApplication a)
        {
            //a.ControlledApplication.DocumentOpened -= new EventHandler<DocumentOpenedEventArgs>(IUpdaterInitialized);
            return Result.Succeeded;
        }
        #endregion

        #region Create all ribbon item: tab, panel, button....
        //Create the buton on the panel
        private void CreateRibbonItem(UIControlledApplication uiApp)
        {
            //Create panel workflow
            var panelWokflow = CreatePanel(uiApp, "TD", "Workflow");

            #region button TrackChange
            // Create splitbutton track change
            var btnDataDMU = new PushButtonData("btnDMU", "Off DMU", assemblyPath, typeof(DMU.CmdFamilyUpdater).FullName);
            btnDataDMU.LargeImage = ImageUtils.ConvertFromBitmap(Resources.ToggleOfLarge);

            var btnDataTrackChange = new PushButtonData("btnTrackChange", "Off TrackChange", assemblyPath, typeof(CmdTrackChange).FullName);
            btnDataTrackChange.LargeImage = ImageUtils.ConvertFromBitmap(Resources.icon);
            SplitButtonData splDataTrackChange = new SplitButtonData("splTrackChange", "TrackChange");
            SplitButton splTrackChange = panelWokflow.AddItem(splDataTrackChange) as SplitButton;

            btnDMU = splTrackChange.AddPushButton(btnDataDMU);
            btnTrackChange = splTrackChange.AddPushButton(btnDataTrackChange);
            #endregion
        }

        #endregion //Create all ribbon item: tab, panel, button

        #region Events
        //Event Track change
        public void app_DocumentOpened(object sender, DocumentOpenedEventArgs args)
        {
            // get document from event args.
            Document doc = args.Document;
            try
            {
                //using (Transaction transaction = new Transaction(doc, "Track changes Start"))
                //{
                //    if (transaction.Start() == TransactionStatus.Started)
                //    {
                //        CmdTrackChange trackChange = new CmdTrackChange();
                //        trackChange.TrackChangesCommand(doc);
                //        transaction.Commit();
                //    }
                //}
            }
            catch { }

        }
        #endregion //Events

        #region Events of IUpdater
        //To run automatically when document opended
        public void IUpdaterInitialized(object sender, DocumentOpenedEventArgs args)
        {
            Application app = sender as Application;
            Document doc = args.Document;

            //try
            //{
            //    using (Transaction transaction = new Transaction(doc, "DMU Start"))
            //    {
            //        if (transaction.Start() == TransactionStatus.Started)
            //        {
            //            DMU.CmdFamilyUpdater fUpdater = new DMU.CmdFamilyUpdater();
            //            fUpdater.UpdaterCommand(app,doc);
            //            transaction.Commit();
            //        }
            //    }
            //}
            //catch
            //{
            //    throw;
            //}
        }

        #endregion //Event of Iupdater


        #region Helper pour create panel    
        public RibbonPanel CreatePanel(UIControlledApplication a, string tabName, string panelName)
        {
            RibbonPanel ribbonPanel = null;
            try
            {
                a.CreateRibbonTab(tabName);
            }
            catch { }
            // Try to create ribbon panel.
            try
            {
                RibbonPanel panel = a.CreateRibbonPanel(tabName, panelName);
            }
            catch { }
            // Search existing tab for your panel.
            List<RibbonPanel> panels = a.GetRibbonPanels(tabName);
            foreach (RibbonPanel p in panels)
            {
                if (p.Name == panelName)
                {
                    ribbonPanel = p;
                }
            }
            return ribbonPanel;
        }
        #endregion //Helper pour create panel

       
    }
}
