using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
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
        public ICommand ColorElementCommand { get; set; }

        public bool radRvtIsChecked { get; set; }
        public bool radActiveViewIsChecked { get; set; }
        public bool radPreSelectedIsChecked { get; set; }
        public bool IsLoaded = false;
        public ObservableCollection<string> ListView { get; set; }
        //public static ObservableCollection<string> ElementListTest { get; set; }

        //public GetElementIn ElementInView { get; set; } //Enum
        private ObservableCollection<ElementId> _elementIdList;

        private bool _radRvtIsChecked;
        private bool _radActiveViewIsChecked;
        private bool _radPreSelectedIsChecked;
        private UIApplication _uiapp = null;

        #region Properties
        public ObservableCollection<ElementId> ElementIdList
        { get => _elementIdList;
            set
            {
                _elementIdList = value;
                OnPropertyChanged();
            }
        }
        public bool IsAllElement
        {
            get => _radRvtIsChecked;
            set
            {
                _radRvtIsChecked = value;
                OnPropertyChanged("Rvt");//Using [CallMemberName], if no it likes: OnPropertyChanged(this, "Rvt")
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
            _uiapp = CmdShowFormWPF.UIAPP;
            #region Get Data
            //ElementRevit root = new ElementRevit();
            //ElementRevit elements = new ElementRevit();  //Give a list element here
            #endregion

            #region Command

            //Command (with external event) to select elements in revit
            SelectElementCommand = new RelayCommand<object>(
               (p) => true,
               (p) =>
               {
                   WindowTest._ExtEvent.Raise();
               });

            //ColorElementCommand = new RelayCommand<object>(
            //  (p) => true,
            //  (p) =>
            //  {
            //      ColorElement();
            //  });
            //Command select the radio button and get the list element
            #endregion

        }
        
    }
}