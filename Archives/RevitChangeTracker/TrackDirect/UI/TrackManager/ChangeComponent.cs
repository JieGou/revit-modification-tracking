using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TrackDirect.Utilities;

namespace TrackDirect.UI
{
    public class ChangeComponent : INotifyPropertyChanged
    {
        public enum ChangeTypeEnum
        {
            NewElement,
            VolumeChange,
            LocationChange,
            DeletedElement
        }

        private static readonly string _pCreatedDateName = VCFParameters.VCF_CreateAt.ToString();
        private static readonly string _pModifiedDateName = VCFParameters.VCF_ModifyAt.ToString();
        private static readonly string _pChangeTypeName = VCFParameters.VCF_ChangeType.ToString();
        private static readonly string _pUserName = VCFParameters.VCF_User.ToString();

        private string name = "";
        private ElementId id = ElementId.InvalidElementId;
        private string categoryName = "None";
        private bool selected = false;
        private string uniqueId = "";
        private string user = "";
        private string dateCreated = "";
        private string dateModified = "";
        private string changeType = "";




        public string Name{ get { return name; } set { name = value; OnPropertyChanged(); } }
        public ElementId Id { get { return id; } set { id = value; OnPropertyChanged(); } }
        public string CategoryName { get { return categoryName; } set { categoryName = value; OnPropertyChanged(); } }
        public bool Selected { get { return selected; } set { selected = value; OnPropertyChanged(); } }
        public string UniqueId { get { return uniqueId; } set { uniqueId = value; OnPropertyChanged(); } }
        public string User { get { return user; } set { user = value; OnPropertyChanged(); } }
        public string DateCreated { get { return dateCreated; } set { dateCreated = value; OnPropertyChanged(); } }
        public string DateModified { get { return dateModified; } set { dateModified = value; OnPropertyChanged(); } }
        public string ChangeType { get { return changeType; } set { changeType = value; OnPropertyChanged(); } }

        public ChangeComponent(Element e)
        {
            try
            {
                name= GetElementFullName(e);
                id = e.Id;
                uniqueId = e.UniqueId;
                categoryName = e.Category.Name;

                var elemFound = ChangeComponentFilter.AllElements.Where(x => x.id == e.Id);
                if (elemFound.Count() > 0)
                {
                    ChangeComponent filteredRvtComponent = elemFound.First();
                    selected = filteredRvtComponent.Selected;
                }

                user = RevitUtils.ParameterToString(e.GetParameters(_pUserName).FirstOrDefault());
                dateCreated = RevitUtils.ParameterToString(e.GetParameters(_pCreatedDateName).FirstOrDefault());
                dateModified = RevitUtils.ParameterToString(e.GetParameters(_pModifiedDateName).FirstOrDefault());
                changeType = RevitUtils.ParameterToString(e.GetParameters(_pChangeTypeName).FirstOrDefault());

            }
            catch { }
        }

       


        #region Methods to get info properties
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
            return $"{categoryName} - {e.Id.IntegerValue} - {familyName} {symbolName} <{e.Name}>";
        }


        #endregion //Methods to get info properties


        public ChangeComponent()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
