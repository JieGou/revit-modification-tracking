using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace TrackChanges
{
    public class HighlightElementEvent : IExternalEventHandler
    {
        private ExternalEvent exEvent;
        private HighlightElementEvent handler;
        private bool _isChecked;

      
        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            CategorySet categories = CategoryUtils.CreateCategoryList(doc, app);
            IList<ElementId>  eleIds = ElementUtils.GetElementList(doc, categories).Select(x => x.Id).ToList();


            uidoc.Selection.SetElementIds(eleIds);
            uidoc.ShowElements(eleIds);


        }


        public string GetName()
        {
            return "Highlight element";
        }
       

    }
}
