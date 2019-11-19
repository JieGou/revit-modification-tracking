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
        internal static RibbonItem btnTrackChange { get; set; }
        internal static App Instance { get; }
            

        #region Result Startup
        public Result OnStartup(UIControlledApplication a)
        {
            CreateRibbonItem(a);
            try
            {
                // Register event. 
                a.ControlledApplication.DocumentOpened += new EventHandler<DocumentOpenedEventArgs>(app_DocumentOpened);
            }
            catch
            {
                return Result.Failed;
            }
            return Result.Succeeded;
        }
        #endregion

        #region Result Shutdown
        public Result OnShutdown(UIControlledApplication a)
        {
            a.ControlledApplication.DocumentOpened -= new EventHandler<DocumentOpenedEventArgs>(app_DocumentOpened);
            return Result.Succeeded;
        }
        #endregion

        #region Create all ribbon item: tab, panel, button....
        //Create the buton on the panel
        private void CreateRibbonItem(UIControlledApplication uiApp)
        {
            //Create panel workflow
            var panelWokflow = CreatePanel(uiApp, "TD", "Workflow");

            // Create splitbutton track change
            var btnDataTrackChange = new PushButtonData("btnTrackChange", "Off Track", assemblyPath, typeof(CmdTrackChange).FullName);
            btnDataTrackChange.LargeImage = ImageUtils.ConvertFromIcon(Resources.icon);
            //btnTrackChange.ToolTip = "Track the modification of model";
            var btnDataTrackSetting = new PushButtonData("btnTrackSetting", "Settings", assemblyPath, typeof(RefTrackChanges).FullName);
            btnDataTrackSetting.LargeImage = ImageUtils.ConvertFromIcon(Resources.icon);
            SplitButtonData splDataTrackChange = new SplitButtonData("splTrackChange", "TrackChange");
            SplitButton splTrackChange = panelWokflow.AddItem(splDataTrackChange) as SplitButton;

            btnTrackChange = splTrackChange.AddPushButton(btnDataTrackChange);
            splTrackChange.AddPushButton(btnDataTrackSetting);

        }

        #endregion //Create all ribbon item: tab, panel, button

        #region Events
        //Event Track change
        public void app_DocumentOpened(object sender, DocumentOpenedEventArgs args)
        {
            // get document from event args.
            Document doc = args.Document;
            try {
                using (Transaction transaction = new Transaction(doc, "Track changes Start"))
                {
                    if (transaction.Start() == TransactionStatus.Started)
                    {
                        CmdTrackChange trackChange = new CmdTrackChange();
                        trackChange.TrackChangesCommand(doc);
                        transaction.Commit();
                    }
                }
            }
            catch { }
            
        }
        #endregion //Events

      

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
