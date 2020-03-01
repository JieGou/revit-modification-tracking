using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TrackDirect.Models;

namespace TrackDirect.UI
{
    public class CompareResultViewModel : BaseViewModel
    {
        #region Data

        bool? _isChecked = false;
        CompareResultViewModel _parent;

        #endregion // Data
        #region Properties
        public ICommand LoadedWindowCommand { get; set; }
        public bool IsLoaded = false;


        public ObservableCollection<CompareResultViewModel> Children { get; private set; }
        private ObservableCollection<RevitTreeItem> _rvtTreeItems;

        public bool IsInitiallySelected { get; private set; }

        public string Name { get; private set; }

        #region constructor
        public CompareResultViewModel()
        {
            _rvtTreeItems = new ObservableCollection<RevitTreeItem>();
            #region Command
            LoadedWindowCommand = new RelayCommand((p) =>{ IsLoaded = true; });
            #endregion //Command
        }
        private CompareResultViewModel(string name)
        {
            this.Name = name;
            this.Children = new ObservableCollection<CompareResultViewModel>();
        }

        #endregion //constructor

        #region CreateFoos

       



        #region methods
        private void Initialize()
        {
            foreach (CompareResultViewModel child in this.Children)
            {
                child._parent = this;
                child.Initialize();
            }
        }
        private void Treeview_Loaded()
        {

            //foreach (DirectoryInfo di in dirInfo.GetDirectories())
            //{

            //    TreeViewItem item = new TreeViewItem();
            //    item.Tag = di;
            //    item.Header = di.Name;

            //    if (di.GetDirectories().Length > 0 || di.GetFiles("*.rfa").Length > 0) item.Items.Add("*");

            //    FamilyTreeList.Items.Add(item);
            //}
            //CreateFamilyItems(dirInfo, FamilyTreeList);
        }

        private void TreeViewItem_Expanded()
        {
            //TreeViewItem item = e.Source as TreeViewItem;
            //if ((item.Items.Count == 1) && (item.Items[0] is string))
            //{
            //    item.Items.Clear();

            //    DirectoryInfo expandedDir = null;
            //    if (item.Tag is DriveInfo)
            //        expandedDir = (item.Tag as DriveInfo).RootDirectory;
            //    if (item.Tag is DirectoryInfo)
            //        expandedDir = (item.Tag as DirectoryInfo);
            //    try
            //    {
            //        foreach (DirectoryInfo subDir in expandedDir.GetDirectories())
            //            item.Items.Add(CreateTreeItem(subDir));
            //    }
            //    catch { }
            //}
        }

        private  void CreateTreeItem(object o)
        {
            //RevitTreeItem item = new RevitTreeItem();
            //item.Header = o.ToString();
            //item.Tag = o;
            //item.Items.Add("Loading...");
            //return item;
        }
        private void AddItemIntoTreeViewItem(ObservableCollection<CompareResultViewModel> root, CompareResultViewModel node)
        {
                root.Add(node);
        }

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

        #endregion // Properties
        #endregion //methods

        #region Get datas for view
        #region Render elements in treeview

        #region render categories

        #endregion //render categories
        #endregion //Render elements in treeview

        //public static List<CompareViewModel> CreateFoos()
        //{
        //    CompareViewModel root = new CompareViewModel("Weapons")
        //    {
        //        IsInitiallySelected = true,
        //        Children =
        //        {
        //            new CompareViewModel("Blades")
        //            {
        //                Children =
        //                {
        //                    new CompareViewModel("Dagger"),
        //                    new CompareViewModel("Machete"),
        //                    new CompareViewModel("Sword"),
        //                }
        //            },
        //            new CompareViewModel("Vehicles")
        //            {
        //                Children =
        //                {
        //                    new CompareViewModel("Apache Helicopter"),
        //                    new CompareViewModel("Submarine"),
        //                    new CompareViewModel("Tank"),
        //                }
        //            },
        //            new CompareViewModel("Guns")
        //            {
        //                Children =
        //                {
        //                    new CompareViewModel("AK 47"),
        //                    new CompareViewModel("Beretta"),
        //                    new CompareViewModel("Uzi"),
        //                }
        //            },
        //        }
        //    };

        //    root.Initialize();
        //    return new List<CompareViewModel> { root };
        //}


        #endregion //Get datas for view
        #endregion // CreateFoos

      
    }
}
