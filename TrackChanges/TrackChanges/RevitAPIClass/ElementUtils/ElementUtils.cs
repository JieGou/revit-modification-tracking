using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

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
    }
}
