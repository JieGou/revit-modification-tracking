using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public bool IsLoaded = false;
        //public GetElementIn ElementInView { get; set; } //Enum
        public ObservableCollection<Element> ElementList;

        private bool _radRvtIsChecked;
        private bool _radActiveViewIsChecked;
        private bool _radPreSelectedIsChecked;

        #region Properties
        public bool IsAllElement
        {
            get => _radRvtIsChecked;
            set
            {
                _radRvtIsChecked = value;
                OnPropertyChanged("Rvt");
            }
        }
        public bool IsElementInActiveView
        {
            get => _radActiveViewIsChecked;
            set
            {
                _radActiveViewIsChecked = value;
                OnPropertyChanged("ActiveView");
            }
        }
        public bool IsElementPreSelected
        {
            get => _radPreSelectedIsChecked;
            set
            {
                _radPreSelectedIsChecked = value;
                OnPropertyChanged("PreSelected");
            }
        }
        #endregion //Properties

        //Provide datacontext and commands for the form
        public MainViewModel()
        {


            #region Get Data
            ElementRevit root = new ElementRevit { IdName = "Element List" };
            ElementRevit elements = new ElementRevit();  //Give a list element here

            #endregion

            #region Command
            //LoadedWindowCommand = new RelayCommand<object>(
            //    (p) => true,
            //    (p) =>
            //    {
            //        IsLoaded = true;
            //        LoginWindow loginWindow = new LoginWindow();
            //        loginWindow.ShowDialog();
            //    });

            //Command (with external event) to select elements in revit
            SelectElementCommand = new RelayCommand<object>(
               (p) => true,
               (p) =>
               {
                   WindowTest._ExtEvent.Raise();
               });

            //Command select the radio button and get the list element
            #endregion

        }

    }
}