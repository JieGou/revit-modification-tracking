using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using System.IO;

namespace TrackChanges
{
    public class CategoryUtils
    {
        public static CategorySet GetCategoriesList(Autodesk.Revit.DB.Document doc, Autodesk.Revit.ApplicationServices.Application app)
        {
            CategorySet myCategorySet = app.Create.NewCategorySet();
            Categories categories = doc.Settings.Categories;
            Category materialCat = categories.get_Item(BuiltInCategory.OST_Materials);

            foreach (Category c in categories)
            {
                if (c.AllowsBoundParameters && c.CategoryType == CategoryType.Model && c != materialCat)
                {
                    myCategorySet.Insert(c);
                }
            }

            return myCategorySet;
        }
        public static CategorySet GetCategoriesHasElements(Autodesk.Revit.DB.Document doc, Autodesk.Revit.ApplicationServices.Application app)
        {
            CategorySet myCategorySet = app.Create.NewCategorySet();
            FilteredElementCollector collector= new FilteredElementCollector(doc);

            collector
              .WhereElementIsNotElementType()
              .WhereElementIsViewIndependent()
              .ToElements();
            foreach (Element element in collector)
            {
                if (element.Category != null
                  && 0 < element.Parameters.Size
                  && (element.Category.HasMaterialQuantities))
                {
                   
                     myCategorySet.Insert(element.Category);
                }
            }
            return myCategorySet;
        }
    }
}
