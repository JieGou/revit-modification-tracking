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
    class AppTestWpf : IExternalApplication
    {
        internal static string assemblyPath = typeof(App).Assembly.Location;
        public static AppTestWpf GetInstance { get; private set; } = null;
        public static PushButton btnTestWpf { get; set; }
        public static WindowTestRequestHandler Handler { get; set; }
        public static ExternalEvent ExEvent { get; set; }




        #region Result Startup
        public Result OnStartup(UIControlledApplication a)
        {
            
            GetInstance = this;  // static access to this application instance
            //Create the ribbon item
            CreateRibbonItem(a);
            Handler = new WindowTestRequestHandler();
            ExEvent = ExternalEvent.Create(Handler);

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
            var panelWpf= CreatePanel(uiApp, "DRTO-VCF", "WPF");

            #region button TrackChange
            // Create splitbutton track change
            var btnData = new PushButtonData("btnTestWpf", "Show WPF", assemblyPath, typeof(CmdShowWindowTest).FullName);
            btnData.LargeImage = ImageUtils.ConvertFromBitmap(Resources.icon);
            btnData.ToolTip = "Show form WPF to test with external event";
            btnTestWpf = panelWpf.AddItem(btnData) as PushButton;
            
            #endregion
        }

        #endregion //Create all ribbon item: tab, panel, button

 
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
