using System;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using System.Reflection;
using System.Runtime.InteropServices;
using Autodesk.Windows;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;
using RibbonPanel = Autodesk.Revit.UI.RibbonPanel;

namespace TD.Core
{
    public class RibbonUtils
    {

        #region Helper pour create panel    
        public static RibbonPanel CreatePanel(UIControlledApplication a, string tabName, string panelName)
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
