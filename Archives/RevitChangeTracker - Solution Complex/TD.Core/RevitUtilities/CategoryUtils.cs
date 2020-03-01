using System.Collections.Generic;
using Autodesk.Revit.DB;
using System.Collections.ObjectModel;

namespace TD.Core
{
    public class CategoryUtils
    {
        /// <summary>
        /// Retrieve all categories defaut of Revit
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        public static CategorySet GetModelCategories(Autodesk.Revit.DB.Document doc, Autodesk.Revit.ApplicationServices.Application app)
        {
            CategorySet myCategorySet = app.Create.NewCategorySet();
            Categories categories = doc.Settings.Categories;
            Category materialCat = categories.get_Item(BuiltInCategory.OST_Materials);

            foreach (Category c in categories)
            {
                if (c.AllowsBoundParameters && c.CategoryType == CategoryType.Model && c != materialCat && c.CanAddSubcategory)
                {
                    myCategorySet.Insert(c);
                }
            }

            return myCategorySet;
        }

        /// <summary>
        /// Retrieve all the categories in which the elements in these categories exist 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        public static CategorySet GetCategoriesHasElements(Autodesk.Revit.DB.Document doc, Autodesk.Revit.ApplicationServices.Application app)
        {
            CategorySet myCategorySet = app.Create.NewCategorySet();
            FilteredElementCollector collector = new FilteredElementCollector(doc);

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

        /// <summary>
        /// Retrieve list category give by developper
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static CategorySet ListCategoryDefaut(
              Document doc, Autodesk.Revit.ApplicationServices.Application app)
        {
            // what categories of family instances
            // are we interested in?

            #region Get all list of BuilInCategories interested
            BuiltInCategory[] bics = new BuiltInCategory[] {
                    //****************Element Architecture******************
                    BuiltInCategory.OST_Ceilings,
                    BuiltInCategory.OST_Columns,
                    BuiltInCategory.OST_Doors,
                    BuiltInCategory.OST_ElectricalEquipment,
                    BuiltInCategory.OST_ElectricalFixtures,
                    BuiltInCategory.OST_Furniture,
                    BuiltInCategory.OST_GenericModel,
                    BuiltInCategory.OST_Mass,
                    BuiltInCategory.OST_Parts,
                    BuiltInCategory.OST_MechanicalEquipment,
                    BuiltInCategory.OST_Parking,
                    BuiltInCategory.OST_Planting,
                    BuiltInCategory.OST_PlumbingFixtures,
                    BuiltInCategory.OST_Railings,
                    BuiltInCategory.OST_Roads,
                    BuiltInCategory.OST_Windows,

                    //****************Element Strutural******************
                    BuiltInCategory.OST_Floors,
                    BuiltInCategory.OST_Ramps,
                    BuiltInCategory.OST_Roofs,
                    BuiltInCategory.OST_FloorOpening,
                    BuiltInCategory.OST_Stairs,
                    BuiltInCategory.OST_StructuralColumns,
                    BuiltInCategory.OST_StructuralFraming,
                    BuiltInCategory.OST_StructuralFoundation,
                    BuiltInCategory.OST_Rebar,
                    BuiltInCategory.OST_Walls

        };
            #endregion

            CategorySet myCategorySet = app.Create.NewCategorySet();
            foreach (var bic in bics)
            {
                try
                {
                    var c = Category.GetCategory(doc, bic);
                    if (c != null)
                        myCategorySet.Insert(c);

                }
                catch { }

            }
            return myCategorySet;
        }


        /// <summary>
        /// Retrives categoryset from list category id
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="app"></param>
        /// <param name="catIds"></param>
        /// <returns></returns>
        public static CategorySet GetCategorySet(Autodesk.Revit.DB.Document doc, Autodesk.Revit.ApplicationServices.Application app, ObservableCollection<ElementId> catIds)
        {
            CategorySet myCategorySet = app.Create.NewCategorySet();
            foreach (var id in catIds)
            {
                Category cat = Category.GetCategory(doc, id);
                if (cat != null) myCategorySet.Insert(cat);

            }
            return myCategorySet;
        }
        public static CategorySet GetCategorySet(Autodesk.Revit.DB.Document doc, Autodesk.Revit.ApplicationServices.Application app, IList<ElementId> catIds)
        {
            CategorySet myCategorySet = app.Create.NewCategorySet();
            foreach (var id in catIds)
            {
                Category cat = Category.GetCategory(doc, id);
                if (cat != null) myCategorySet.Insert(cat);

            }
            return myCategorySet;
        }
    }
}
