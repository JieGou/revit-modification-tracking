using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace TrackChanges
{
    public class ElementStrutural
    {
        /// <summary>
        /// Get only structural elements
        /// </summary>
        FilteredElementCollector GetStructuralElements(
                Document doc)
            {
                // what categories of family instances
                // are we interested in?

                BuiltInCategory[] bics = new BuiltInCategory[] {
                    
                    //Element Architecture
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

                    //Element strutural
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

                    //Element Mechanical, electrical, piping
                    //BuiltInCategory.OST_CableTray,
                    BuiltInCategory.OST_CableTrayFitting,
                    //BuiltInCategory.OST_Conduit,
                    BuiltInCategory.OST_ConduitFitting,
                    //BuiltInCategory.OST_DuctCurves,
                    BuiltInCategory.OST_DuctFitting,
                    BuiltInCategory.OST_DuctTerminal,
                    BuiltInCategory.OST_ElectricalEquipment,
                    BuiltInCategory.OST_ElectricalFixtures,
                    BuiltInCategory.OST_LightingDevices,
                    BuiltInCategory.OST_LightingFixtures,
                    BuiltInCategory.OST_MechanicalEquipment,
                    //BuiltInCategory.OST_PipeCurves,
                    BuiltInCategory.OST_PipeFitting,
                    BuiltInCategory.OST_PlumbingFixtures,
                    BuiltInCategory.OST_SpecialityEquipment,
                    BuiltInCategory.OST_Sprinklers,
                    //BuiltInCategory.OST_Wire,


                };

                IList<ElementFilter> a = new List<ElementFilter>(bics.Count());

                foreach (BuiltInCategory bic in bics)
                {
                    a.Add(new ElementCategoryFilter(bic));
                }

                LogicalOrFilter categoryFilter = new LogicalOrFilter(a);

                LogicalAndFilter familyInstanceFilter
                    = new LogicalAndFilter(categoryFilter,new ElementClassFilter(typeof(FamilyInstance)));

                IList<ElementFilter> b = new List<ElementFilter>(6);

                b.Add(new ElementClassFilter(typeof(Wall)));
                b.Add(new ElementClassFilter(typeof(Floor)));
                b.Add(new ElementClassFilter(typeof(PointLoad)));
                b.Add(new ElementClassFilter(typeof(LineLoad)));
                b.Add(new ElementClassFilter(typeof(AreaLoad)));
                b.Add(familyInstanceFilter);

                LogicalOrFilter classFilter = new LogicalOrFilter(b);

                FilteredElementCollector collector = new FilteredElementCollector(doc);

                collector.WherePasses(classFilter);

                return collector;
            }
            //Categories cats = doc.Settings.Categories;

            //List<ElementFilter> a = new List<ElementFilter>();

            //foreach (Category c in cats)
            //{
            //    if (CategoryType.Model == c.CategoryType)
            //    {
            //        a.Add(new ElementCategoryFilter(c.Id));
            //    }
            //}

            //ElementFilter isModelCategory = new LogicalOrFilter(a);

            //Options opt = new Options();

            //return new FilteredElementCollector(doc)
            //    .WhereElementIsNotElementType()
            //    .WhereElementIsViewIndependent()
            //    .WherePasses(isModelCategory)
            //    .Where<Element>(e =>
            //        (null != e.get_BoundingBox(null))
            //        && (null != e.get_Geometry(opt)));
        
    }
}
