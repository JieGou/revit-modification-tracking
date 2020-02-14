using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Threading.Tasks;
using System.Windows;

namespace TrackChanges
{
    public class WindowTestViewModel : ViewModelBase
    {
        public RelayCommand SelectElementCommand { get; set; }
        public RelayCommand ColorElementCommand { get; set; }
        public RelayCommand UnColorElementCommand { get; set; }
        //public RelayCommand<WindowTest> GetData { get; set; }

        //public RelayCommand<WindowTest> ModelSelection { get; set; }

        public RelayCommand<WindowTest> GetTreeView { get; set; }


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

        #region Initialize form
        public WindowTestViewModel()
        {
            Model = new WindowTestModel();
            #region Command

            //Command (with external event) to select elements in revit

            SelectElementCommand = new RelayCommand(OnSelectElement);
            ColorElementCommand = new RelayCommand(OnColorElement);
            UnColorElementCommand = new RelayCommand(OnUnColorElement);
            GetTreeView = new RelayCommand<WindowTest>(OnGetTreeView);
            #endregion

        }
        #endregion //Initialize form



        #region External command, retrieve form model

        //Provide data when form loaded
        private async void OnWindowLoaded(Window win)
        {
            //Task<User> userTask = Model.GetUser();
            //Task<List<ProjectManagement.Models.Project>> projects = Model.GetUserProjectsAsync();
            //Documents = _uiapp.Application.Documents;

            //User = await userTask;
            //Projects = await projects;
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
        public async void OnGetTreeView(WindowTest win)
        {
            Model.GetTreeView();
        }
        #endregion //External command, retrieve form model




    }
}