#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using ApiViet.Properties;
using ApiViet.Ribbon;
using ApiViet.Learning;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

#endregion

namespace TrackChanges
{
    /// <summary>
    /// Create a ribbon tab
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class AppStartProject : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication uiApp)
        {
            CustomRibbon ribbon = new CustomRibbon(uiApp);
            var myTab = ribbon.Tab("TD");
            var panelLearning = myTab.Panel("Learning");
            var btn1 = panelLearning
                .CreateButton("btnInfoElement",
                    "Info\nElement",
                    typeof(CmdPickMultiObjects),
                    btn => btn
                        .SetLargeImage(Properties.Resources.icon)
                        .SetSmallImage(Properties.Resources.icon)
                        .SetContextualHelp(ContextualHelpType.Url, "https://help.autodesk.com"));
            return  Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
