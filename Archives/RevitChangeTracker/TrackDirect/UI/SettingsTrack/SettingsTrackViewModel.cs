using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using static TrackDirect.UI.AutoTrackDataStorageUtil;

namespace TrackDirect.UI
{
    public class SettingsTrackViewModel : BaseViewModel
    {
        #region RelayCommand
        public RelayCommand TrackViewLoaded { get; set; }
        public RelayCommand TrackViewClosing { get; set; }
        public RelayCommand OkCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand BtnSelectAllCommand { get; set; }
        public RelayCommand BtnSelectNoneCommand { get; set; }
        public RelayCommand SaveProject_Checked { get; set; }
        public RelayCommand SynchronizeProject_Checked { get; set; }
        public RelayCommand OpenProject_Checked { get; set; }
        public RelayCommand SwitchProject_Checked { get; set; }
        public RelayCommand RadioBtnCanRun_Checked { get; set; }
        public RelayCommand RadioBtnDoNotRun_Checked { get; set; }
        public RelayCommand ItemListBox_Checked { get; set; }
        public RelayCommand ByTime_Checked { get; set; }

        #endregion //Relay command

        #region Properties for itemControl in view
        private bool canRunAutoTrack = false;
        private bool isAutoRunSaveProject = false;
        private bool isAutoRunSynchroProject = false;
        private bool isAutoRunOpenProject = false;
        private bool isAutoRunSwitchProject = false;
        private bool isAutoRunByTime = false;
        private int timeOut = 10; //Minutes
        private int countSelectedCategory = 0;

        public bool CanRunAutoTrack { get { return canRunAutoTrack; } set { canRunAutoTrack = value; OnPropertyChanged(); } }
        public bool DoNotRunAutoTrack { get { return !canRunAutoTrack; } set { canRunAutoTrack = !value; OnPropertyChanged(); } }
        public bool IsAutoRunSaveProject { get { return isAutoRunSaveProject; } set { isAutoRunSaveProject = value; OnPropertyChanged(); } }
        public bool IsAutoRunSynchroProject { get { return isAutoRunSynchroProject; } set { isAutoRunSynchroProject = value; OnPropertyChanged(); } }
        public bool IsAutoRunOpenProject { get { return isAutoRunOpenProject; } set { isAutoRunOpenProject = value; OnPropertyChanged(); } }
        public bool IsAutoRunSwitchProject  { get { return isAutoRunSwitchProject; } set { isAutoRunSwitchProject = value; OnPropertyChanged(); } }
        public bool IsAutoRunByTime { get { return isAutoRunByTime; } set { isAutoRunByTime= value; OnPropertyChanged(); } }
        public int TimeOut { get { return timeOut; } set { timeOut = value; OnPropertyChanged(); } }
        public int CountSelectedCategory { get { return countSelectedCategory; } set { countSelectedCategory = value; OnPropertyChanged(); } }
        #endregion //Properties for itemControl in view

        private UIApplication _uiapp = null;
        private Document _doc;
        private AutoTrackSettings toolSettings = null;
        private ObservableCollection<CategoryProperties> selectedCategories = new ObservableCollection<CategoryProperties>();
        private ObservableCollection<CategoryProperties> categories = new ObservableCollection<CategoryProperties>();

       
        public AutoTrackSettings ToolSettings { get { return toolSettings; } set { toolSettings = value; OnPropertyChanged(); } }
        public ObservableCollection<CategoryProperties> Categories { get { return categories; } set { categories = value; OnPropertyChanged(); } }
        public ObservableCollection<CategoryProperties> SelectedCategories { get { return selectedCategories; } set { selectedCategories = value; OnPropertyChanged(); } }
        
        public bool IsLoaded = false;

        public SettingsTrackViewModel(UIApplication uiapp)
        {
            _uiapp = uiapp;
            _doc = _uiapp.ActiveUIDocument.Document;
            try
            {
                TrackViewLoaded = new RelayCommand(param => this.OnTrackViewLoaded(param));
                TrackViewClosing = new RelayCommand(param => this.OnTrackViewClosing(param));
                OkCommand = new RelayCommand(param => this.OnOkCommand(param));
                CancelCommand = new RelayCommand(param => this.OnCancelCommand(param));
                BtnSelectAllCommand = new RelayCommand(param => this.OnSelectAllCommand(param));
                BtnSelectNoneCommand = new RelayCommand(param => this.OnSelectNoneCommand(param));
                RadioBtnCanRun_Checked = new RelayCommand(param => this.OnRadioBtnCanRun_Checked(param));
                RadioBtnDoNotRun_Checked = new RelayCommand(param => this.OnRadioBtnDoNotRun_Checked(param));
                SaveProject_Checked = new RelayCommand(param => this.OnSaveProject_Checked(param));
                SynchronizeProject_Checked = new RelayCommand(param => this.OnSynchronizeProject_Checked(param));
                OpenProject_Checked = new RelayCommand(param => this.OnOpenProject_Checked(param));
                SwitchProject_Checked = new RelayCommand(param => this.OnSwitchProject_Checked(param));
                ByTime_Checked = new RelayCommand(param => this.OnByTime_Checked(param));
                ItemListBox_Checked = new RelayCommand(param => this.OnItemListBox_Checked(param));
               

            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
           
        }

        #region Methods for events and commands

        /// <summary>
        /// TrackView Loaded
        /// </summary>
        /// <param name="param"></param>
        private void OnTrackViewLoaded(object param)
        {
            IsLoaded = true;
            
        }
        /// <summary>
        /// TrackView Closed
        /// </summary>
        /// <param name="param"></param>
        private void OnTrackViewClosing(object param)
        {
            
        }
        
        /// <summary>
        /// Change category set
        /// </summary>
        /// <param name="param"></param>
        private void OnOkCommand(Object param)
        {
            Window win = param as Window;
            win.DialogResult = true;
            try
            {
                if (SetToolSettings())
                {
                    if (AutoTrackDataStorageUtil.StoreAutoTrackCreatorSettings(_doc, toolSettings)) { };//Save settings
                    if (CategoryDataStorageUtil.StoreCategoryProperties(_doc, categories)) { };
                    if(!canRunAutoTrack && CmdAutoTrack.IsRunning) //If in Wpf set DoNotRun Auto, but CmdAutoTrack is running, we need stop it
                    {
                        CmdAutoTrack cmdAutoTrack = new CmdAutoTrack();
                        if(_uiapp != null)
                            cmdAutoTrack.Execute(_uiapp);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "AutoTrack Creator: Save Settings", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        /// <summary>
        /// Change category set
        /// </summary>
        /// <param name="param"></param>
        private void OnCancelCommand(object param)
        {
            SettingTrackView view = param as SettingTrackView;
            if (view != null)
            {
               view.Close();
            }
        }
        private void OnSelectAllCommand(object param)
        {
            try
            {
                for (int i = 0; i < categories.Count; i++)
                {
                    categories[i].Selected = true;
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }
        private void OnSelectNoneCommand(object param)
        {
            try
            {
                for (int i = 0; i < categories.Count; i++)
                {
                    categories[i].Selected = false;
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }

        #region Event checkbox
        private void OnSwitchProject_Checked(object param)
        {
            if (null != toolSettings)
            {
                toolSettings.IsAutoTrackEventViewActive = canRunAutoTrack && isAutoRunSwitchProject ? true : false;
            }
        }

        private void OnOpenProject_Checked(object param)
        {
            if (null != toolSettings)
            {
                toolSettings.IsAutoTrackEventDocumentOpen = canRunAutoTrack && isAutoRunOpenProject ? true : false;
            }
        }

        private void OnSynchronizeProject_Checked(object param)
        {
            if (null != toolSettings)
            {
                toolSettings.IsAutoTrackEventSynchronize = canRunAutoTrack && isAutoRunSynchroProject ? true : false;
            }
        }

        private void OnSaveProject_Checked(object param)
        {
            if (null != toolSettings)
            {
                toolSettings.IsAutoTrackEventSave = canRunAutoTrack && isAutoRunSaveProject ? true : false;
            }
        }
        private void OnByTime_Checked(object param)
        {
            if (null != toolSettings)
            {
                toolSettings.IsAutoTrackByTime = canRunAutoTrack && isAutoRunByTime ? true : false;
            }
        }
        private void OnRadioBtnDoNotRun_Checked(object param)
        {
            if (null != toolSettings)
            {
                toolSettings.CanAutoRun = canRunAutoTrack;
            }
            this.CanRunAutoTrack = false;
        }

        private void OnRadioBtnCanRun_Checked(object param)
        {
            if (null != toolSettings)
            {
                toolSettings.CanAutoRun = canRunAutoTrack;
            }
            this.CanRunAutoTrack = true;
            this.IsAutoRunByTime = true;
            this.IsAutoRunSaveProject = true;
            this.IsAutoRunSwitchProject = true;
            this.IsAutoRunSynchroProject = true;
            this.IsAutoRunOpenProject = true;
        }
        private void OnItemListBox_Checked(object param)
        {
          CountSelectedCategory = categories.Where(x => x.Selected == true).Count();
        }

        #endregion //Event Checkbox
        #endregion //Command


        #region Method Get Settings Data storage
        public bool DisplayUI()
        {
            bool result = false;
            try
            {
                bool displayedSettings = DisplaySettings();

                result = displayedSettings;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to display UI components\n" + ex.Message, "Display UI", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return result;
        }

        private bool DisplaySettings()
        {
            bool result = false;
            try
            {
                GetToolSettings();

                result = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to display tools settings.\n" + ex.Message, "AutoTrack: DisplaySettings", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return result;
        }


        /// <summary>
        /// Get setting from data storage of Element Database in Revit
        /// </summary>
        private void GetToolSettings()
        {
            try
            {
                toolSettings = AutoTrackDataStorageUtil.GetAutoTrackCreatorSettings(_doc);
                var temp = CategoryDataStorageUtil.GetCategoryPropertiesDataStorage(_doc);
                ObservableCollection<CategoryProperties> categoryList = new ObservableCollection<CategoryProperties>();
                foreach(var c in temp)
                {
                    categoryList.Add(c);
                }
               
                if(categoryList.Count < 1)
                {
                    categoryList = ComponentCategoryFilter.GetCategoryProperties(_doc); 
                }
               
                ComponentCategoryFilter.UpdateCategoryList(categoryList);
                categories = ComponentCategoryFilter.Categories;
                //selectedCategories = ComponentCategoryFilter.SelectedCategories;
                countSelectedCategory = categories.Where(x => x.Selected == true).Count();

                this.CanRunAutoTrack = toolSettings.CanAutoRun;
                if (toolSettings.CanAutoRun)
                {
                    canRunAutoTrack = toolSettings.CanAutoRun;
                   isAutoRunSaveProject = toolSettings.IsAutoTrackEventSave;
                    isAutoRunSynchroProject = toolSettings.IsAutoTrackEventSynchronize;
                    isAutoRunOpenProject = toolSettings.IsAutoTrackEventDocumentOpen;
                    isAutoRunSwitchProject = toolSettings.IsAutoTrackEventViewActive;
                    isAutoRunByTime = toolSettings.IsAutoTrackByTime;
                }
                timeOut = toolSettings.TimeOut <= 1 ? 10 : toolSettings.TimeOut;//If time out not valid, take defaut 10 minutes
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to get AutoTrack settings.\n" + ex.Message, "AutoTrack: GetToolSettings", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Set new data storage in Element Database Revit
        /// </summary>
        /// <returns></returns>
        private bool SetToolSettings()
        {
            bool result = false;
            try
            {
                toolSettings.CanAutoRun = canRunAutoTrack;
                toolSettings.IsAutoTrackEventSave = isAutoRunSaveProject;
                toolSettings.IsAutoTrackEventSynchronize = isAutoRunSynchroProject;
                toolSettings.IsAutoTrackEventDocumentOpen = isAutoRunOpenProject;
                toolSettings.IsAutoTrackEventViewActive = isAutoRunSwitchProject;
                toolSettings.IsAutoTrackByTime = isAutoRunByTime;
                toolSettings.TimeOut = timeOut >= 1 ? timeOut: 10 ;
                result = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save the settings.\n" + ex.Message, "AutoTrack Creator: SetToolSettings", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return result;
        }
        #endregion //Method Get Data storage

    }
}
