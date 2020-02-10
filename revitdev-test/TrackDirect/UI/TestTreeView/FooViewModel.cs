using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using TrackDirect.Utilities;

using TrackDirect.Models;
using System.Runtime.CompilerServices;

namespace TrackDirect.UI
{
    public class FooViewModel : BaseViewModel
    {
        #region Declarations
        public RelayCommand WindowLoaded { get; set; }

        public bool IsLoaded = false;
        public FooModel fooModel { get; set; }
        public ObservableCollection<FooViewModel> Children { get; private set; }
        public bool IsInitiallySelected { get; private set; }
        public string Name { get; private set; }
        private ObservableCollection<RevitTreeItem> _rvtTreeItems;
        #endregion // Declarations

        #region Data
        bool? _isChecked = false;
        FooViewModel _parent;
        #endregion // Data


        #region Properties

        #endregion //Properties
        public ObservableCollection<RevitTreeItem> RevitTreeItems {
            get => _rvtTreeItems;
            set
            {
                _rvtTreeItems = value;
                OnPropertyChanged("Revit");
            }
        }

        public FooViewModel()
        {
            _rvtTreeItems = new ObservableCollection<RevitTreeItem>();
            fooModel = new FooModel();

            WindowLoaded = new RelayCommand(param => this.OnTrackViewLoaded(param));


        }
        #region constructor
        private FooViewModel(string name)
        {
            this.Name = name;
            this.Children = new ObservableCollection<FooViewModel>();
        }

        #endregion //constructor

        #region CreateFoos
        //Provide data when form loaded
        private void OnWindowLoaded(Window1 win)
        {
            //win.tree.ItemsSource = FooRequestHandler.GetElementItems();


        }

        private void TreeViewItem_Expanded()
        {
           
        }

        private void CreateTreeItem()
        {
            
        }

        private void OnTrackViewLoaded(object param)
        {

        }
        public static ObservableCollection<FooViewModel> GetElementItems()
        {

            return null;
        }


        public static ObservableCollection<FooViewModel> CreateFoos()
        {
            FooViewModel root = new FooViewModel("Weapons")
            {
                IsInitiallySelected = true,
                Children =
                {
                    new FooViewModel("Blades")
                    {
                        Children =
                        {
                            new FooViewModel("Dagger"),
                            new FooViewModel("Machete"),
                            new FooViewModel("Sword"),
                        }
                    },
                    new FooViewModel("Vehicles")
                    {
                        Children =
                        {
                            new FooViewModel("Apache Helicopter"),
                            new FooViewModel("Submarine"),
                            new FooViewModel("Tank"),                            
                        }
                    },
                    new FooViewModel("Guns")
                    {
                        Children =
                        {
                            new FooViewModel("AK 47"),
                            new FooViewModel("Beretta"),
                            new FooViewModel("Uzi"),
                        }
                    },
                }
            };

            root.Initialize();
            return new ObservableCollection<FooViewModel>{ root };
        }

        void Initialize()
        {
            foreach (FooViewModel child in this.Children)
            {
                child._parent = this;
                child.Initialize();
            }
        }

        #endregion // CreateFoos

        #region IsChecked

        /// <summary>
        /// Gets/sets the state of the associated UI toggle (ex. CheckBox).
        /// The return value is calculated based on the check state of all
        /// child FooViewModels.  Setting this property to true or false
        /// will set all children to the same check state, and setting it 
        /// to any value will cause the parent to verify its check state.
        /// </summary>
        public bool? IsChecked
        {
            get { return _isChecked; }
            set { this.SetIsChecked(value, true, true); }
        }

        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _isChecked)
                return;

            _isChecked = value;

            if (updateChildren && _isChecked.HasValue)
                this.Children.ToList().ForEach(c => c.SetIsChecked(_isChecked, true, false));

            if (updateParent && _parent != null)
                _parent.VerifyCheckState();

            this.OnPropertyChanged("IsChecked");
        }

        void VerifyCheckState()
        {
            bool? state = null;
            for (int i = 0; i < this.Children.Count; ++i)
            {
                bool? current = this.Children[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }
            this.SetIsChecked(state, false, true);
        }

        #endregion // IsChecked

    }
}