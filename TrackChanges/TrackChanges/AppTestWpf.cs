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
        public static AppTestWpf Instance { get; private set; } = null;

        public static RibbonItem btnTestWpf { get; set; }

        //Declare the Wpf form
        private WindowTest _wpfForm;



        #region Result Startup
        public Result OnStartup(UIControlledApplication a)
        {
            _wpfForm = null;   // no dialog needed yet; the command will bring it
            Instance = this;  // static access to this application instance

            //Create the ribbon item
            CreateRibbonItem(a);

          
            return Result.Succeeded;
        }
        #endregion

        #region Result Shutdown
        public Result OnShutdown(UIControlledApplication a)
        {
            //a.ControlledApplication.DocumentOpened -= new EventHandler<DocumentOpenedEventArgs>(IUpdaterInitialized);

            //dispose window wpf after using
            if (_wpfForm != null && _wpfForm.IsVisible)
                _wpfForm.Close();

            return Result.Succeeded;
        }
        #endregion

        #region Create all ribbon item: tab, panel, button....
        //Create the buton on the panel
        private void CreateRibbonItem(UIControlledApplication uiApp)
        {
            //Create panel workflow
            var panelWpf= CreatePanel(uiApp, "TD", "WPF");

            #region button TrackChange
            // Create splitbutton track change
            var btnData = new PushButtonData("btnTestWpf", "Show WPF", assemblyPath, typeof(CmdShowFormWPF).FullName);
            btnData.LargeImage = ImageUtils.ConvertFromBitmap(Resources.icon);
            btnData.ToolTip = "Show form WPF to test with external event";
            btnTestWpf = panelWpf.AddItem(btnData);

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

        #region Show form modeless dialog wpf with all the external events
        public void ShowForm(UIApplication uiapp)
        {
            //If we do not have a dialog yet, create and show it
            //We do not have the garbage collection in wpf
            //https://stackoverflow.com/questions/39334444/equivalent-to-isdisposed-using-c-sharp-wpf

            if (_wpfForm is null)
            {
                #region Initialize all the external event for wpf form
                // A new handler to handle request posting by the dialog
                HighlightElementExEvent highlight = new HighlightElementExEvent();
                GetListElementsExEvent getListElement = new GetListElementsExEvent();
                #endregion //Initialize all the external event for wpf form

                #region Decla

                #endregion
                // External Event for the dialog to use (to post requests)
                ExternalEvent exEvent = ExternalEvent.Create(highlight); //--> using for rasing event

                // We give the objects to the new dialog;
                // The dialog becomes the owner responsible for disposing them, eventually.
                _wpfForm = new WindowTest(exEvent, highlight);
                _wpfForm.Show();

            }
        }
        #endregion
    }
}
