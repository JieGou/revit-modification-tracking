using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;


namespace TrackChanges
{
    public class WindowTestViewModel : ViewModelBase
    {
        public RelayCommand SelectElementCommand { get; set; }
        public RelayCommand ColorElementCommand { get; set; }
        public RelayCommand UnColorElementCommand { get; set; }
        public WindowTestModel Model { get; set; }
        public ObservableCollection<string> ListView { get; set; }
        private ObservableCollection<ElementId> _elementIdList;
        public ObservableCollection<MenuTreeItem> TreeItems;
        public ObservableCollection<string> ViewItems { get; set; }

        private bool _radRvtIsChecked;
        private bool _radActiveViewIsChecked;
        private bool _radPreSelectedIsChecked;


        #region Properties
        public ObservableCollection<ElementId> ElementIdList
        { get => _elementIdList;
            set
            {
                _elementIdList = value;
                RaisePropertyChanged(() => ElementIdList);
            }
        }
        public bool IsAllElement
        {
            get => _radRvtIsChecked;
            set
            {
                _radRvtIsChecked = value;
                RaisePropertyChanged(() => IsAllElement);
            }
        }
        public bool IsElementInActiveView
        {
            get => _radActiveViewIsChecked;
            set
            {
                _radActiveViewIsChecked = value;
                RaisePropertyChanged(() => IsElementInActiveView);
            }
        }
        public bool IsElementPreSelected
        {
            get => _radPreSelectedIsChecked;
            set
            {
                _radPreSelectedIsChecked = value;
                RaisePropertyChanged(() => IsElementPreSelected);
            }
        }
        #endregion //Properties

        //Provide datacontext and commands for the form
        public WindowTestViewModel()
        {
            #region Command

            //Command (with external event) to select elements in revit
            Model = new WindowTestModel();
            SelectElementCommand = new RelayCommand(OnSelectElement);
            ColorElementCommand = new RelayCommand(OnColorElement);
            UnColorElementCommand = new RelayCommand(OnUnColorElement);
            #endregion

        }

        public void OnSelectElement()
        {
            Model.SelectElement();
        }
        public void OnColorElement()
        {
            Model.ColorElement();
        }
        public void OnUnColorElement()
        {
            Model.UnColorElement();
        }

        

    }
}