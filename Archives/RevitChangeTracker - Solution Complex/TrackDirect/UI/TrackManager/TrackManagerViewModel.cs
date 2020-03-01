using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TrackDirect.Models;
using System.Drawing;
using TrackDirect.Utilities;
using System.Windows.Media;
using Visibility = System.Windows.Visibility;
using System.IO;
using System.Collections.Generic;

namespace TrackDirect.UI
{
    public class TrackManagerViewModel : BaseViewModel
    {
        #region RelayCommand
        public RelayCommand TrackManagerWindowLoaded { get; set; }
        public RelayCommand TrackManagerWindowClosed { get; set; }
        public RelayCommand ColorSettingsNewElement { get; set; }
        public RelayCommand ColorSettingsChangeFamilyType { get; set; }
        public RelayCommand ColorSettingsChangeGeometry { get; set; }
        public RelayCommand ColorSettingsChangeVolume { get; set; }
        public RelayCommand ColorSettingsChangeRevitParameters { get; set; }
        public RelayCommand ColorSettingsChangeSharedParameters { get; set; }
        public RelayCommand RadBtnByCategory_Checked { get; set; }
        public RelayCommand RadBtnByTime_Checked { get; set; }
        public RelayCommand RadBtnByTypeChange_Checked { get; set; }
        public RelayCommand ApplyCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }


        #endregion //Relay command


        private static UIApplication _uiapp = null;
        private Document _doc = null;
        private bool isLoaded = false;
        private Autodesk.Revit.DB.Color rvtColorNewElement;
        private Autodesk.Revit.DB.Color rvtColorChangeFamilyType;
        private Autodesk.Revit.DB.Color rvtColorChangeGeometry;
        private Autodesk.Revit.DB.Color rvtColorChangeVolume;
        private Autodesk.Revit.DB.Color rvtColorChangeRvtPara;
        private Autodesk.Revit.DB.Color rvtColorChangeSharedPara;
        private bool isHigLight = true;
        private bool isColorElement = true;
        private bool isIsolate = false;
        private ComponentOption selectOption = ComponentOption.OnlyVisible;
        private int tabSelectIndex = 0;
        private TrackManagerHandler _handler = null;

        public Document Doc { get { return _doc; } set { _doc = value; } }
        public static UIApplication Uiapp { get { return _uiapp; } set { _uiapp = value; } }
        public Autodesk.Revit.DB.Color RvtColorNewElement { get { return rvtColorNewElement; } set { rvtColorNewElement = value; OnPropertyChanged(); } }
        public Autodesk.Revit.DB.Color RvtColorChangeFamilyType { get { return rvtColorChangeFamilyType; } set { rvtColorChangeFamilyType = value; OnPropertyChanged(); } }
        public Autodesk.Revit.DB.Color RvtColorChangeGeometry { get { return rvtColorChangeGeometry; } set { rvtColorChangeGeometry = value; OnPropertyChanged(); } }
        public Autodesk.Revit.DB.Color RvtColorChangeVolume { get { return rvtColorChangeVolume; } set { rvtColorChangeVolume = value; OnPropertyChanged(); } }
        public Autodesk.Revit.DB.Color RvtColorChangeRvtPara { get { return rvtColorChangeRvtPara; } set { rvtColorChangeRvtPara = value; OnPropertyChanged(); } }
        public Autodesk.Revit.DB.Color RvtColorChangeSharedPara { get { return rvtColorChangeSharedPara; } set { rvtColorChangeSharedPara = value; OnPropertyChanged(); } }
        public bool IsHighLight { get { return isHigLight; } set { isHigLight = value; OnPropertyChanged(); } }
        public bool IsColorElement { get { return isColorElement; } set { isColorElement = value; OnPropertyChanged(); } }
        public bool IsIsolate { get { return isIsolate; } set { isIsolate = value; OnPropertyChanged(); } }
        public ComponentOption SelectOption { get { return selectOption; } set { selectOption = value; OnPropertyChanged(); } }
        public int TabSelectIndex { get { return tabSelectIndex; } set { tabSelectIndex = value; OnPropertyChanged(); } }
        public static string ColorSettingsFiles = string.Empty;
        public static ColorSettings _ColorSettings { get; set; }

        public bool IsLoaded { get { return isLoaded; } set { isLoaded = value; OnPropertyChanged(); } }
        public static bool IsOpen { get; private set; }


        #region Properties binding;
        private ObservableCollection<TreeElementModel> treeElementsByCategory = new ObservableCollection<TreeElementModel>();
        private ObservableCollection<TreeElementModel> treeElementsByDate = new ObservableCollection<TreeElementModel>();
        private ObservableCollection<TreeElementModel> treeElementsByTypeChange = new ObservableCollection<TreeElementModel>();
        private ObservableCollection<TreeElementModel> treeElementsActive = new ObservableCollection<TreeElementModel>();

        public ObservableCollection<TreeElementModel> TreeElementsByCategory { get { return treeElementsByCategory; } set { treeElementsByCategory = value; OnPropertyChanged(); } }
        public ObservableCollection<TreeElementModel> TreeElementsByDate { get { return treeElementsByDate; } set { treeElementsByDate = value; OnPropertyChanged(); } }
        public ObservableCollection<TreeElementModel> TreeElementsByTypeChange { get { return treeElementsByTypeChange; } set { treeElementsByTypeChange = value; OnPropertyChanged(); } }
        public ObservableCollection<TreeElementModel> TreeElementsActive { get { return treeElementsActive; } set { treeElementsActive = value; OnPropertyChanged(); } }


        #endregion //Properties biding


        public TrackManagerViewModel(UIApplication uiapp, TrackManagerHandler handler)
        {
            _uiapp = uiapp;
            _doc = _uiapp.ActiveUIDocument.Document;
            _handler = handler;


            try
            {
                TrackManagerWindowLoaded = new RelayCommand(param => this.WindowLoadedExecuted(param));
                TrackManagerWindowClosed = new RelayCommand(param => this.WindowClosedExecuted(param));
                ApplyCommand = new RelayCommand(param => this.ApplyExecuted(param));
                CancelCommand = new RelayCommand(param => this.CancelExecuted(param));
                ColorSettingsNewElement = new RelayCommand(param => this.ColorNewElementExecuted(param));
                ColorSettingsChangeFamilyType = new RelayCommand(param => this.ColorChangeFamilyExecuted(param));
                ColorSettingsChangeGeometry = new RelayCommand(param => this.ColorChangeGeoExecuted(param));
                ColorSettingsChangeVolume = new RelayCommand(param => this.ColorChangeVolumeExecuted(param));
                ColorSettingsChangeRevitParameters = new RelayCommand(param => this.ColorRvtParaExecuted(param));
                ColorSettingsChangeSharedParameters = new RelayCommand(param => this.ColorSharedParaExecuted(param));
                RadBtnByCategory_Checked = new RelayCommand(param => this.OnradBtnByCategory_Checked(param));
                RadBtnByTime_Checked = new RelayCommand(param => this.OnradBtnByTime_Checked(param));
                RadBtnByTypeChange_Checked = new RelayCommand(param => this.OnradBtnByTypeChange_Checked(param));

            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }

        }

        #region Methods for relay command
        private void WindowLoadedExecuted(object param)
        {
            IsLoaded = true;
            IsOpen = true;


        }
        private void WindowClosedExecuted(object param)
        {
            IsOpen = false;
        }
        private void ApplyExecuted(object param)
        {
            if (tabSelectIndex == 0) //Tab remove color
            {
                RaiseApplyView();
            }
            if (tabSelectIndex == 1) //Tab remove color
            {
                RaiseRemoveColor();
            }
        }
        private void CancelExecuted(object param)
        {
            var win = param as Window;
            win.Close();
        }

        private void ColorNewElementExecuted(object param)
        {
            //Model.ColorSettings();
            ColorSelectionDialog colorSelectionDialog = new ColorSelectionDialog();
            colorSelectionDialog.Show();
            RvtColorNewElement = colorSelectionDialog.SelectedColor;
            _ColorSettings.NewElement = RvtColorNewElement;

        }
        private void ColorChangeFamilyExecuted(object param)
        {
            //Model.ColorSettings();
            ColorSelectionDialog colorSelectionDialog = new ColorSelectionDialog();
            colorSelectionDialog.Show();
            RvtColorChangeFamilyType = colorSelectionDialog.SelectedColor;
            _ColorSettings.ChangeFamilyType = RvtColorChangeFamilyType;
        }
        private void ColorChangeGeoExecuted(object param)
        {
            //Model.ColorSettings();
            ColorSelectionDialog colorSelectionDialog = new ColorSelectionDialog();
            colorSelectionDialog.Show();
            RvtColorChangeGeometry = colorSelectionDialog.SelectedColor;
            _ColorSettings.ChangeGeometry = RvtColorChangeGeometry;
        }
        private void ColorChangeVolumeExecuted(object param)
        {
            //Model.ColorSettings();
            ColorSelectionDialog colorSelectionDialog = new ColorSelectionDialog();
            colorSelectionDialog.Show();
            RvtColorChangeVolume = colorSelectionDialog.SelectedColor;
            _ColorSettings.ChangeVolumeLocation = RvtColorChangeVolume;
        }
        private void ColorRvtParaExecuted(object param)
        {
            //Model.ColorSettings();
            ColorSelectionDialog colorSelectionDialog = new ColorSelectionDialog();
            colorSelectionDialog.Show();
            RvtColorChangeRvtPara = colorSelectionDialog.SelectedColor;
            _ColorSettings.ChangeRvtPara = RvtColorChangeRvtPara;
        }
        private void ColorSharedParaExecuted(object param)
        {
            //Model.ColorSettings();
            ColorSelectionDialog colorSelectionDialog = new ColorSelectionDialog();
            colorSelectionDialog.Show();
            RvtColorChangeSharedPara = colorSelectionDialog.SelectedColor;
            _ColorSettings.ChangeSharedPara = RvtColorChangeSharedPara;
        }

        private void OnradBtnByCategory_Checked(object param)
        {
            TreeElementsActive = new ObservableCollection<TreeElementModel>(TreeElementsByCategory);
        }
        private void OnradBtnByTime_Checked(object param)
        {
            TreeElementsActive = new ObservableCollection<TreeElementModel>(TreeElementsByDate);
        }
        private void OnradBtnByTypeChange_Checked(object param)
        {
            TreeElementsActive = new ObservableCollection<TreeElementModel>(TreeElementsByTypeChange);
        }

        public void RaiseApplyView()
        {
            _handler.ViewModel = this;
            AppCommand.ManageHandler.Request.Make(TrackManagerHandler.RequestId.ApplyView);
            AppCommand.ManageExEvent.Raise();
            AppCommand.SetFocusToRevit();
        }

        public void RaiseRemoveColor()
        {
            _handler.ViewModel = this;
            AppCommand.ManageHandler.Request.Make(TrackManagerHandler.RequestId.RemoveColor);
            AppCommand.ManageExEvent.Raise();
            AppCommand.SetFocusToRevit();
        }

        #endregion //Command

        #region Public methods
        public bool DisplayUI()
        {
            bool result = false;
            try
            {
                CollectProperties();
                CollectSettingsJson();
                CollectElement();

                result = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to display UI components\n" + ex.Message, "TrackManager: DisplayUI", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return result;
        }
        #endregion //Public methods

        #region private methods
        private void CollectProperties()
        {
            //SelectOption = ComponentOption.OnlyVisible;
        }
       
        private void CollectElement()
        {
            try
            {

                var temp = CategoryDataStorageUtil.GetCategoryPropertiesDataStorage(_doc).Where(x => x.Selected == true); 
                IList<CategoryProperties> categoryList = new List<CategoryProperties>(temp);
                
                if (categoryList.Count < 1)
                {
                    categoryList = new List<CategoryProperties>(ComponentCategoryFilter.GetCategoryProperties(_doc));
                }
                
                var catIdList = categoryList.Select(x => x.CategoryId).ToList();
               
                var elems = ElementUtils.GetElementsByCategories(_doc, catIdList);
                ChangeComponentFilter.AllElements.Clear();
                ChangeComponentFilter.NewElements.Clear();
                ChangeComponentFilter.ModifiedElements.Clear();
                ChangeComponentFilter.UnClassifiedElements.Clear();

                foreach (var e in elems)
                {
                    if (e != null)
                    {
                        var change = new ChangeComponent(e);
                        ChangeComponentFilter.AllElements.Add(change);
                        if (change.ChangeType == ChangedElement.ChangeTypeEnum.NewElement.ToString())
                        {
                            //New Elements
                            if (string.IsNullOrEmpty(change.DateCreated)) change.DateCreated = "None";
                            ChangeComponentFilter.NewElements.Add(change);

                        }
                        else if (!string.IsNullOrWhiteSpace(change.DateModified) && !string.IsNullOrWhiteSpace(change.ChangeType))
                        {
                            //Modified Elements
                            if (string.IsNullOrEmpty(change.DateModified)) change.DateModified = "None";
                            ChangeComponentFilter.ModifiedElements.Add(change);
                        }
                        else
                        {
                            ChangeComponentFilter.UnClassifiedElements.Add(change);
                        }
                    }
                }
                treeElementsByCategory = TreeElementModel.RenderByCategory();
                treeElementsByDate = TreeElementModel.RenderByDate();
                treeElementsByTypeChange = TreeElementModel.RenderByTypeChange();
                treeElementsActive = new ObservableCollection<TreeElementModel>(treeElementsByCategory);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Collect Element.\n" + ex.Message, "TrackManager: CollectElement", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion

        #region private methods

        private void CollectSettingsJson()
        {
            ColorSettingsFiles = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Settings\\ColorSettings.json";
            _ColorSettings = JsonUtils.Load<ColorSettings>(ColorSettingsFiles);

            //Get Color Revit form color defaut of wpf
            var cNewElement = System.Drawing.Color.Red;

            RvtColorNewElement = _ColorSettings.NewElement?? new Autodesk.Revit.DB.Color(cNewElement.R, cNewElement.G, cNewElement.B);
            RvtColorChangeFamilyType = _ColorSettings.ChangeFamilyType ?? new Autodesk.Revit.DB.Color(cNewElement.R, cNewElement.G, cNewElement.B);
            RvtColorChangeGeometry = _ColorSettings.ChangeGeometry ?? new Autodesk.Revit.DB.Color(cNewElement.R, cNewElement.G, cNewElement.B);
            RvtColorChangeVolume = _ColorSettings.ChangeVolumeLocation ?? new Autodesk.Revit.DB.Color(cNewElement.R, cNewElement.G, cNewElement.B);
            RvtColorChangeRvtPara = _ColorSettings.ChangeRvtPara ?? new Autodesk.Revit.DB.Color(cNewElement.R, cNewElement.G, cNewElement.B);
            RvtColorChangeSharedPara = _ColorSettings.ChangeSharedPara ?? new Autodesk.Revit.DB.Color(cNewElement.R, cNewElement.G, cNewElement.B);

        }
        
        #endregion
    }
    }
