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
        public static CategorySet CreateCategoryList(Autodesk.Revit.DB.Document doc, Autodesk.Revit.ApplicationServices.Application app)
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
    }
}
