using Autodesk.Revit.DB;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TrackDirect.UI
{
    /// <summary>
    /// Revit Category Properties
    /// </summary>
    public class CategoryProperties : INotifyPropertyChanged
    {
        private string categoryName = "";
        private ElementId categoryId = ElementId.InvalidElementId;
        private bool selected = true;

        public string CategoryName { get { return categoryName; } set { categoryName = value; OnPropertyChanged("CategoryName"); } }
        public ElementId CategoryId { get { return categoryId; } set { categoryId = value; OnPropertyChanged("CategoryId"); } }
        public bool Selected { get { return selected; } set { selected = value; OnPropertyChanged("Selected"); } }

        public CategoryProperties(Category category)
        {
            try
            {
                categoryName = category.Name;
                categoryId = category.Id;
                var categoryFound = from cat in ComponentCategoryFilter.Categories where cat.CategoryId == categoryId select cat;
                if (categoryFound.Count() > 0)
                {
                    CategoryProperties filteredCategory = categoryFound.First();
                    selected = filteredCategory.Selected;
                }
            }
            catch{}
        }
       
        public CategoryProperties()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}
