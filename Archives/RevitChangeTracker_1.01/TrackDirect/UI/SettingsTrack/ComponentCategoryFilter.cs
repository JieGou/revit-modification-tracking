using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackDirect.Utilities;

namespace TrackDirect.UI
{

    public class ComponentCategoryFilter
    {

        private static ObservableCollection<CategoryProperties> categories = new ObservableCollection<CategoryProperties>();
        private static ObservableCollection<CategoryProperties> selectedCategories = new ObservableCollection<CategoryProperties>();

        public static ObservableCollection<CategoryProperties> Categories { get { return categories; } set { categories = value; } }
        public static ObservableCollection<CategoryProperties> SelectedCategories { get { return selectedCategories; } set { selectedCategories = value; } }

        
        public static void UpdateCategoryList(ObservableCollection<CategoryProperties> catList)
        {
            selectedCategories.Clear();
            try
            {
                foreach (CategoryProperties cp in catList)
                {
                    var found = from cat in categories where cat.CategoryId == cp.CategoryId select cat;
                    if (found.Count() == 0)
                    {
                        categories.Add(cp);
                    }
                    if (cp.Selected)
                    {
                        selectedCategories.Add(cp);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }

        /// <summary>
        /// Get CategoryPropeties list in document
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static ObservableCollection<CategoryProperties> GetCategoryProperties(Document doc)
            {
                var catList = CategoryUtils.ListCategoryDefaut(doc);
                var catProperties = new ObservableCollection<CategoryProperties>();
                foreach (var cat in catList)
                {
                    if (cat != null)
                    {
                        var catProperty = new CategoryProperties(cat);
                        catProperties.Add(catProperty);
                    }
                }
                return catProperties;
            }
        }
    }
