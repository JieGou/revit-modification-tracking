using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;

namespace TrackDirect
{
    public class ElementUtils
    {
        #region Retrieve elements of interest
        /// <summary>
        /// Retrieve all elements to track.
        /// It is up to you to decide which elements
        /// are of interest to you.
        /// </summary>
        public static IList<Element> GetTrackedElements(
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
                                                       // Créer filtre de plusieurs catégorie
                                                       //ELE
                    BuiltInCategory.OST_ElectricalEquipment,
                    BuiltInCategory.OST_ElectricalFixtures,
                    BuiltInCategory.OST_LightingDevices,
                    BuiltInCategory.OST_DataDevices,
                    BuiltInCategory.OST_CommunicationDevices,
                    BuiltInCategory.OST_FireAlarmDevices,
                    BuiltInCategory.OST_SecurityDevices,
                    BuiltInCategory.OST_Wire
        };
            #endregion

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

            return collector.WherePasses(filter)
                .WhereElementIsViewIndependent()
                .WhereElementIsNotElementType().ToElements();

        }

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
