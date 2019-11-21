using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace TrackChanges
{
    public class MainViewModel : BaseViewModel
    {
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand SelectElementCommand { get; set; }
        public bool radRvtIsChecked { get; set; }
        public bool radActiveViewIsChecked { get; set; }
        public bool radPreSelectedIsChecked { get; set; }
        public GetElementIn ElementInView { get; set; }

        public bool IsLoaded = false;
        public MainViewModel()
        {
            //LoadedWindowCommand = new RelayCommand<object>(
            //    (p) => true,
            //    (p) =>
            //    {
            //        IsLoaded = true;
            //        LoginWindow loginWindow = new LoginWindow();
            //        loginWindow.ShowDialog();
            //    });

            SelectElementCommand = new RelayCommand<object>(
                (p) => true,
                (p) =>
                {
                    WindowTest._ExtEvent.Raise();
                });
        }


        #region check radio button : element in view

        #endregion
        bool _radRvtIsChecked;
        public bool IsAllElement
        {
            get => _radRvtIsChecked;
            set
            {
                _radRvtIsChecked = value;
                OnPropertyChanged("Rvt");
            }
        }

        bool _radActiveViewIsChecked;
        public bool IsElementInActiveView
        {
            get => _radActiveViewIsChecked;
            set
            {
                _radActiveViewIsChecked = value;
                OnPropertyChanged("ActiveView");
            }
        }

        bool _radPreSelectedIsChecked;
        public bool IsElementPreSelected
        {
            get => _radPreSelectedIsChecked;
            set
            {
                _radPreSelectedIsChecked = value;
                OnPropertyChanged("PreSelected");
            }
        }

        #region GetElement in project
        public IList<Element> GetElements(Document doc, Autodesk.Revit.ApplicationServices.Application app)
        {
            CategorySet categories = new CategorySet();
            categories = CategoryUtils.CreateCategoryList(doc, app);

            IList<Element> eList = new List<Element>();

            switch (true)
            {
                case true when IsAllElement:
                    eList = ElementUtils.GetElementList(doc, categories);
                    break;
                case true when IsElementInActiveView:
                    eList = ElementUtils.GetElementList(doc, categories, doc.ActiveView);
                    break;
                case true when IsElementPreSelected:
                    eList = ElementUtils.GetElementPreSelected(doc);
                    break;
                default:
                    break;
            }
            return eList;
        }
        #endregion


      

    }
}