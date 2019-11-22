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
    public class GetListElementsExEvent : IExternalEventHandler
    {
        private ExternalEvent exEvent;
        private GetListElementsExEvent handler;
        private bool _isAllElement;
        private bool _isActiveView;
        private bool _isPreSelected;

        
        public void Execute(UIApplication uiapp)
        {
            MainViewModel vmodel = new MainViewModel();
            _isAllElement = vmodel.IsAllElement;
            _isAllElement = vmodel.IsElementInActiveView;
            _isPreSelected = vmodel.IsElementPreSelected;

            Application app = uiapp.Application;
            Document doc = uiapp.ActiveUIDocument.Document;
            CategorySet categories = new CategorySet();
            categories = CategoryUtils.CreateCategoryList(doc, app);

            IList<Element> eList = new List<Element>();

            switch (true)
            {
                case true when _isAllElement:
                    eList = ElementUtils.GetElementList(doc, categories);
                    break;
                case true when _isAllElement:
                    eList = ElementUtils.GetElementList(doc, categories, doc.ActiveView);
                    break;
                case true when _isPreSelected:
                    eList = ElementUtils.GetElementPreSelected(doc);
                    break;
                default:
                    break;
            }


        }

        public string GetName()
        {
            return "Get List Element";
        }

    }
}
