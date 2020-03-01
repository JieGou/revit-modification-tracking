using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;

namespace TD.Core
{
    public class ElementUtils
    {
        #region Retrieve elements of interest
        /// <summary>
        /// Retrieve all elements to track.
        /// It is up to you to decide which elements
        /// are of interest to you.
        /// </summary>
        public static IList<Element> GetElementByCategories(
            Document doc, IList<BuiltInCategory> bics)
        {
            ElementMulticategoryFilter multiCatFilter = new ElementMulticategoryFilter(bics);
            var elems = new FilteredElementCollector(doc).WhereElementIsNotElementType().WherePasses(multiCatFilter).ToElements();
            IList<Element> elements = new List<Element>();
            foreach (Element element in elems)
            {
                if (null != element.Category
                  && 0 < element.Parameters.Size
                && element.Category.HasMaterialQuantities)
                {
                    elements.Add(element);
                }
            }
            return elements;
        }
        #endregion // Retrieve elements of interest

        #region Get all model elements 

        //Get only element has parameter
        public static IList<Element> GetElementInstanceInProject(Document doc, bool isActiveView = false)
        {
            IList<Element> elements = new List<Element>();
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            Options opt = new Options();
            if (isActiveView)
                collector = new FilteredElementCollector(doc, doc.ActiveView.Id);
            collector
                    .WhereElementIsNotElementType()
                    .WhereElementIsViewIndependent()
                    .Where<Element>(e =>
                   (null != e.get_BoundingBox(null))
                   && (null != e.get_Geometry(opt)));


            foreach (Element element in collector)
            {
                if (null != element.Category
                  && 0 < element.Parameters.Size
                && element.Category.HasMaterialQuantities)
                {
                    elements.Add(element);
                }
            }
            return elements;

        }
        #endregion //Get all model elements by category set


      
        public static IList<Element> GetElementsByCategories(Document doc, CategorySet categories)
        {
            //Retrive all model elements
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<ElementFilter> categoryFilters = new List<ElementFilter>();
            IList<Element> elements = new List<Element>();

            foreach (Category category in categories)
            {
                categoryFilters.Add(new ElementCategoryFilter(category.Id));
            }

            ElementFilter filter = new LogicalOrFilter(categoryFilters);
            Options opt = new Options();
            collector.WherePasses(filter)
                .WhereElementIsNotElementType()
                    .WhereElementIsViewIndependent()
                    .Where<Element>(e =>
                   (null != e.get_BoundingBox(null))
                   && (null != e.get_Geometry(opt)));

            foreach (Element element in collector)
            {
                if (null != element.Category
                  && 0 < element.Parameters.Size)
                {
                    elements.Add(element);
                }
            }
            return elements;
        }
        #region Get all model elemennts by categories givens
        public static IList<Element> GetElementsByCategories(Document doc, IList<ElementId> categories)
        {
            //Retrive all model elements
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<ElementFilter> categoryFilters = new List<ElementFilter>();
            IList<Element> elements = new List<Element>();

            foreach (var id in categories)
            {
                categoryFilters.Add(new ElementCategoryFilter(id));
            }

            ElementFilter filter = new LogicalOrFilter(categoryFilters);
            Options opt = new Options();
            collector.WherePasses(filter)
                .WhereElementIsNotElementType()
                    .WhereElementIsViewIndependent()
                    .Where<Element>(e =>
                   (null != e.get_BoundingBox(null))
                   && (null != e.get_Geometry(opt)));

            foreach (Element element in collector)
            {
                if (null != element.Category
                  && 0 < element.Parameters.Size)
                {
                    elements.Add(element);
                }
            }
            return elements;
        }

        public static IList<Element> GetElementsByCategories(Document doc, ElementId catId)
        {
            //Retrive all model elements
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            var categoryFilters = new List<ElementFilter>()
            {
                new ElementCategoryFilter(catId)
            };
             
            ElementFilter filter = new LogicalOrFilter(categoryFilters);
            Options opt = new Options();
            collector.WherePasses(filter)
                .WhereElementIsNotElementType()
                    .WhereElementIsViewIndependent()
                    .Where<Element>(e =>
                   (null != e.get_BoundingBox(null))
                   && (null != e.get_Geometry(opt)))
                  ;
            return collector.ToElements();

            
        }
        #endregion //Get all model elements inv iew acives



        /// <summary>
        /// Get pre-selected elements
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static IList<Element> GetElementPreSelected(Document doc)
        {
            UIDocument uidoc = new UIDocument(doc);
            Selection sel = uidoc.Selection;
            bool isPreSelected = IsPreSelectedElement(doc);
            if (isPreSelected)
                return sel.GetElementIds().Select(x => doc.GetElement(x)).ToList();
            return new List<Element>();
        }
       

        /// <summary>
        /// Detect if in selecting in Revit
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static bool IsPreSelectedElement(Document doc)
        {
            try
            {
                UIDocument uidoc = new UIDocument(doc);
                Selection sel = uidoc.Selection;
                ICollection<ElementId> selectedIds = uidoc.Selection.GetElementIds();
                if (selectedIds.Count > 0) return true;
            }
            catch
            {
                return false;
            }
            return false;
        }
        

    }
}
