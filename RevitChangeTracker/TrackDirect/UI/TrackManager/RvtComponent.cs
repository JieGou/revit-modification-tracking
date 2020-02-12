using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TrackDirect.UI.TrackManager
{
    public class RvtComponent : INotifyPropertyChanged
    {
        private string name = "";
        private ElementId id = ElementId.InvalidElementId;
        private bool selected = false;
        private string UniqueId { get; set; }




        public string Name{ get { return name; } set { name = value; OnPropertyChanged(); } }
        public ElementId Id { get { return id; } set { id = value; OnPropertyChanged(); } }
        public bool Selected { get { return selected; } set { selected = value; OnPropertyChanged(); } }

        public RvtComponent(Element e)
        {
            try
            {
                Name= GetElementFullName(e);
                Id = e.Id;

                var elemFound = RvtComponentFilter.AllElements.Where(x => x.id == e.Id);
                if (elemFound.Count() > 0)
                {
                    RvtComponent filteredRvtComponent = elemFound.First();
                    selected = filteredRvtComponent.Selected;
                }
            }
            catch { }
        }

        public RvtComponent()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        //Get Category, type, family, id and element name and return it as a chaine of string
        private static string GetElementFullName(Element e)
        {
            if (null == e)
            {
                return "<null>";
            }

            // For a wall, the element name equals the
            // wall type name, which is equivalent to the
            // family name ...

            FamilyInstance fi = e as FamilyInstance;

            string typeName = e.GetType().Name;

            string categoryName = (null == e.Category)
              ? string.Empty
              : e.Category.Name;

            string familyName = (null == fi)
              ? string.Empty
              : fi.Symbol.Family.Name;

            string symbolName = (null == fi
              || e.Name.Equals(fi.Symbol.Name))
                ? string.Empty
                : fi.Symbol.Name;
            return $"{categoryName} - {e.Id.IntegerValue} - {typeName} {familyName} {symbolName} <{e.Name}>";
        }
    }
}
