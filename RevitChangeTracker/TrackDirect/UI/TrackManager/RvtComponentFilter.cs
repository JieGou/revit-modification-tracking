using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackDirect.UI.TrackManager
{
    public class RvtComponentFilter
    {
        private static ObservableCollection<RvtComponent> newElements = new ObservableCollection<RvtComponent>();
        private static ObservableCollection<RvtComponent> modifiedElements = new ObservableCollection<RvtComponent>();
        private static ObservableCollection<RvtComponent> unClassifiedElements = new ObservableCollection<RvtComponent>();
        private static ObservableCollection<RvtComponent> allElements = new ObservableCollection<RvtComponent>();

        public static ObservableCollection<RvtComponent> NewElements { get { return newElements; } set { newElements = value; } }
        public static ObservableCollection<RvtComponent> ModifiedElements { get { return modifiedElements; } set { modifiedElements = value; } }
        public static ObservableCollection<RvtComponent> UnClassifiedElements { get { return unClassifiedElements; } set { unClassifiedElements = value; } }
        public static ObservableCollection<RvtComponent> AllElements { get { return allElements; } set { allElements = value; } }

        public static void UpdateNewElementList(ObservableCollection<RvtComponent> elementList)
        {
            foreach (var e in elementList)
            {
                newElements.Add(e);
            }
        }
        public static void UpdateModifiedElementList(ObservableCollection<RvtComponent> elementList)
        {
            foreach (var e in elementList)
            {
                modifiedElements.Add(e);
            }
        }
        public static void UpdateUnClassiffiedElementList(ObservableCollection<RvtComponent> elementList)
        {
            foreach (var e in elementList)
            {
                unClassifiedElements.Add(e);
            }
        }
        public static void UpdateAllElements(ObservableCollection<RvtComponent> elementList)
        {
            foreach (var e in elementList)
            {
                allElements.Add(e);
            }
        }


        /// <summary>
        /// Get CategoryPropeties list in document
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static ObservableCollection<RvtComponent> GetRvtComponents(Document doc)
        {
            var catIdList = CategoryUtils.ListCategoryDefaut(doc).Select(x => x.Id).ToList();
            var elems = ElementUtils.GetElementsByCategories(doc, catIdList);
            var rvtComponents = new ObservableCollection<RvtComponent>();
            foreach (var e in elems)
            {
                if (e != null)
                {
                    var catProperty = new RvtComponent(e);
                    rvtComponents.Add(catProperty);
                }
            }
            return rvtComponents;
        }
    }
}

