using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackDirect.UI
{
    public class ChangeComponentFilter
    {
        private static ObservableCollection<ChangeComponent> newElements = new ObservableCollection<ChangeComponent>();
        private static ObservableCollection<ChangeComponent> modifiedElements = new ObservableCollection<ChangeComponent>();
        private static ObservableCollection<ChangeComponent> unClassifiedElements = new ObservableCollection<ChangeComponent>();
        private static ObservableCollection<ChangeComponent> allElements = new ObservableCollection<ChangeComponent>();

        public static ObservableCollection<ChangeComponent> NewElements { get { return newElements; } set { newElements = value; } }
        public static ObservableCollection<ChangeComponent> ModifiedElements { get { return modifiedElements; } set { modifiedElements = value; } }
        public static ObservableCollection<ChangeComponent> UnClassifiedElements { get { return unClassifiedElements; } set { unClassifiedElements = value; } }
        public static ObservableCollection<ChangeComponent> AllElements { get { return allElements; } set { allElements = value; } }

        public static void UpdateNewElementList(ObservableCollection<ChangeComponent> elementList)
        {
            foreach (var e in elementList)
            {
                newElements.Add(e);
            }
        }
        public static void UpdateModifiedElementList(ObservableCollection<ChangeComponent> elementList)
        {
            foreach (var e in elementList)
            {
                modifiedElements.Add(e);
            }
        }
        public static void UpdateUnClassiffiedElementList(ObservableCollection<ChangeComponent> elementList)
        {
            foreach (var e in elementList)
            {
                unClassifiedElements.Add(e);
            }
        }
        public static void UpdateAllElements(ObservableCollection<ChangeComponent> elementList)
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
        public static ObservableCollection<ChangeComponent> GetRvtComponents(Document doc)
        {
            var catIdList = CategoryUtils.ListCategoryDefaut(doc).Select(x => x.Id).ToList();
            var elems = ElementUtils.GetElementsByCategories(doc, catIdList);
            var changeComponents = new ObservableCollection<ChangeComponent>();
            foreach (var e in elems)
            {
                if (e != null)
                {
                    var catProperty = new ChangeComponent(e);
                    changeComponents.Add(catProperty);
                }
            }
            return changeComponents;
        }
    }
}

