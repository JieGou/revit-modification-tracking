using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;

namespace TrackChanges
{
    public class ElementUtils
    {
        #region Retrieve elements of interest
        /// <summary>
        /// Retrieve all elements to track.
        /// It is up to you to decide which elements
        /// are of interest to you.
        /// </summary>
        public static IEnumerable<Element> GetTrackedElements(
            Document doc)
        {
            Categories cats = doc.Settings.Categories;

            List<ElementFilter> a = new List<ElementFilter>();

            foreach (Category c in cats)
            {
                if (CategoryType.Model == c.CategoryType)
                {
                    a.Add(new ElementCategoryFilter(c.Id));
                }
            }

            ElementFilter isModelCategory = new LogicalOrFilter(a);

            Options opt = new Options();

            return new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .WhereElementIsViewIndependent()
                .WherePasses(isModelCategory)
                .Where<Element>(e =>
                    (null != e.get_BoundingBox(null))
                    && (null != e.get_Geometry(opt)));
        }
        #endregion // Retrieve elements of interest

        #region Get all model elements 
        public static IList<Element> GetElementList(Document doc, CategorySet categories)
        {
            //Retrive all model elements
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<ElementFilter> categoryFilters = new List<ElementFilter>();

            foreach (Category category in categories)
            {
                categoryFilters.Add(new ElementCategoryFilter(category.Id));
            }

            ElementFilter filter = new LogicalOrFilter(categoryFilters);

            return collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();

        }
        #endregion //Get all model elements

        #region Get all model elemennts in view actives
        public static IList<Element> GetElementList(Document doc, CategorySet categories, View activeView)
        {
            //Retrive all model elements
            FilteredElementCollector collector = new FilteredElementCollector(doc, activeView.Id);
            IList<ElementFilter> categoryFilters = new List<ElementFilter>();

            foreach (Category category in categories)
            {
                categoryFilters.Add(new ElementCategoryFilter(category.Id));
            }

            ElementFilter filter = new LogicalOrFilter(categoryFilters);

            return collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();

        }
        #endregion //Get all model elements inv iew acives


        #region Get elements pre-selection

        #endregion //Get elements pre-selection
        public static IList<Element> GetElementList(ExternalCommandData commandData)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            bool isPreSelected = IsPreSelectedElement(commandData);
            if (isPreSelected)
                return sel.GetElementIds().Select(x => doc.GetElement(x)).ToList();
            return new List<Element>();
        }
        #region Check if selection elements is active

        
        public static bool IsPreSelectedElement(ExternalCommandData commandData)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
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
        #endregion //Check if selection elements is active
    }
}
