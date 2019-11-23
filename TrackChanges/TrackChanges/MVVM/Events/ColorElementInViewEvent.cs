using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace TrackChanges
{
    public class ColorElementInViewEvent : IExternalEventHandler
    {
        public ExternalEvent ExEvent;
        private ColorElementInViewEvent handler;

        public void Execute(UIApplication app)
        {
           
        }

        public string GetName()
        {
            throw new System.NotImplementedException();
        }
        public void ColorElement(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;
            CategorySet categories = CategoryUtils.GetCategoriesHasElements(doc, app);
            IList<ElementId> eleIds = ElementUtils.GetElementList(doc, categories).Select(x => x.Id).ToList();



            uidoc.Selection.SetElementIds(eleIds);
            try
            {
                Color color = new Color(0, 255, 255);
                OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                var patternCollector = new FilteredElementCollector(doc.ActiveView.Document);
                patternCollector.OfClass(typeof(Autodesk.Revit.DB.FillPatternElement));
                FillPatternElement solidFill = patternCollector
                    .ToElements()
                    .Cast<Autodesk.Revit.DB.FillPatternElement>()
                    .First(x => x.GetFillPattern().IsSolidFill);


                ogs.SetProjectionLineColor(color); // or other here
                ogs.SetHalftone(true);
                ogs.SetProjectionLineColor(color);
#pragma warning disable CS0618 // Type or member is obsolete
                ogs.SetProjectionFillPatternId(solidFill.Id);
#pragma warning restore CS0618 // Type or member is obsolete
                using (Transaction t = new Transaction(doc, "Set Element Override"))
                {
                    t.Start();
                    foreach (var id in eleIds)
                    {
                        doc.ActiveView.SetElementOverrides(id, ogs);
                    }
                    t.Commit();
                }
            }
            catch 
            {

                throw;
            }

            uidoc.ShowElements(eleIds);
        }
    }
}
