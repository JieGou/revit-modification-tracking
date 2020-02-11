using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using System.Collections.ObjectModel;

namespace TrackDirect
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

        /// <summary>
        /// REtrieve list category give by developper
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<Category> ListCategoryDefaut(
              Document doc)
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
                    BuiltInCategory.OST_Walls,


                     //****************Element SYSTEMES******************
                    BuiltInCategory.OST_DuctCurves,//Gaine
                    BuiltInCategory.OST_PlaceHolderDucts,// espace réservés aux gaines---VERIFIE
                    BuiltInCategory.OST_DuctFitting,//raccords de gaine---VERIFIE
                    BuiltInCategory.OST_DuctAccessory,//accessoir de gaineOST_FlexDuctCurves---VERIFIE
                    BuiltInCategory.OST_FlexDuctCurves,//gaine flexible---VERIFIE
                    BuiltInCategory.OST_DuctTerminal,//Bouche d'aération ---VERIFIE
                    BuiltInCategory.OST_PipeCurves, //Canalisation---VERIFIE
                    BuiltInCategory.OST_PipeFitting,//Raccord de canalisation---VERIFIE
                    BuiltInCategory.OST_PlumbingFixtures,//Appareil sanitairesOST_Sprinklers---VERIFIE
                    BuiltInCategory.OST_PlumbingFixtures,//OST_Sprinklers---VERIFIE
                    BuiltInCategory.OST_CableTray,// chemin de câble---VERIFIE
                    BuiltInCategory.OST_Conduit,//Conduits ***Note fait jusqu'à raccordement chemin à câble---VERIFIE
                    BuiltInCategory.OST_CableTrayFitting,//Raccord chemin de câble
                    BuiltInCategory.OST_LightingFixtures,//Luminaires
            
                    BuiltInCategory.OST_MechanicalEquipment, //Equipement génie climatique
                    BuiltInCategory.OST_PipeAccessory,// asscessoir de canalisation
                                                     
                    BuiltInCategory.OST_ElectricalEquipment,
                    BuiltInCategory.OST_ElectricalFixtures,
                    BuiltInCategory.OST_LightingDevices,
                    BuiltInCategory.OST_DataDevices,
                    BuiltInCategory.OST_CommunicationDevices,
                    BuiltInCategory.OST_FireAlarmDevices,
                    BuiltInCategory.OST_SecurityDevices                  
        };
            #endregion

            List<Category> categories = new List<Category>();
           foreach(var bic in bics)
            {
                try
                {
                    var c = Category.GetCategory(doc, bic);
                    if(c!= null)
                    categories.Add(c);
                }
                catch {}
               
            }
            
            return categories.OrderBy(x => x.Name).ToList();
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
            foreach(var id in catIds)
            {
                    Category cat = Category.GetCategory(doc,id);
                if (cat != null) myCategorySet.Insert(cat);
                
            }
            return myCategorySet;
        }
        public static CategorySet GetCategorySet(Autodesk.Revit.DB.Document doc, Autodesk.Revit.ApplicationServices.Application app,IList<ElementId> catIds)
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
