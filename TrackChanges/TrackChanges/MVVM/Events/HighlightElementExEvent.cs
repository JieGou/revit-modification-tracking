using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace TrackChanges
{
    public class HighlightElementExEvent : IExternalEventHandler
    {
        private ExternalEvent exEvent;
        private HighlightElementExEvent handler;
        private bool _isChecked;

      
        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            IList<ElementId>  eleIds = ElementUtils.GetElementInProject(doc).Select(x => x.Id).ToList();
            uidoc.Selection.SetElementIds(eleIds);
            uidoc.ShowElements(eleIds);


        }


        public string GetName()
        {
            return "Highlight element";
        }
       

    }
}
