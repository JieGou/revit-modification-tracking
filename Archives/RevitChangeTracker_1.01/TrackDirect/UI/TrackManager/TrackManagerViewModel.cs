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
        public RelayCommand ColorSettingsChangeRevitParameters { get; set; }
        public RelayCommand ColorSettingsChangeSharedParameters { get; set; }
        public RelayCommand radBtnByCategory_Checked { get; set; }
        public RelayCommand radBtnByTime_Checked { get; set; }
        public RelayCommand radBtnByTypeChange_Checked { get; set; }


        #endregion //Relay command


        private static UIApplication _uiapp = null;
        private Document _doc = null;
        private bool isLoaded = false;
        private System.Windows.Media.Brush backgroundNewElement = System.Windows.Media.Brushes.Red;
        private System.Windows.Media.Brush backgroundFamilyType = System.Windows.Media.Brushes.Blue;
        private System.Windows.Media.Brush backgroundGeometry = System.Windows.Media.Brushes.Aqua;
        private System.Windows.Media.Brush backgroundRevitParameters = System.Windows.Media.Brushes.Orange;
        private System.Windows.Media.Brush backgroundSharedParameters = System.Windows.Media.Brushes.Violet;
        public static Autodesk.Revit.DB.Color RvtColorNewElement { get; set; }
        public static Autodesk.Revit.DB.Color RvtColorChangeFamilyType { get; set; }
        public static Autodesk.Revit.DB.Color RvtColorChangeGeometry { get; set; }
        public static Autodesk.Revit.DB.Color RvtColorChangeRevitParameters { get; set; }
        public static Autodesk.Revit.DB.Color RvtColorChangeSharedParameters { get; set; }

        public Document Doc { get { return _doc; } set { _doc = value; } }
        public static UIApplication Uiapp { get { return _uiapp; } set { _uiapp = value; } }
        public System.Windows.Media.Brush BackgroundNewElement { get { return backgroundNewElement; } set { backgroundNewElement = value; OnPropertyChanged("BackgroundNewElement"); } }
        public System.Windows.Media.Brush BackgroundFamilyType { get { return backgroundFamilyType; } set { backgroundFamilyType = value; OnPropertyChanged("BackgroundFamilyType"); } }
        public System.Windows.Media.Brush BackgroundGeometry { get { return backgroundGeometry; } set { backgroundGeometry = value; OnPropertyChanged("BackgroundGeometry"); } }
        public System.Windows.Media.Brush BackgroundRevitParameters { get { return backgroundRevitParameters; } set { backgroundRevitParameters = value; OnPropertyChanged("BackgroundRevitParameters"); } }
        public System.Windows.Media.Brush BackgroundSharedParameters { get { return backgroundSharedParameters; } set { backgroundSharedParameters = value; OnPropertyChanged("BackgroundSharedParameters"); } }


        public bool IsLoaded { get { return isLoaded; } set { isLoaded = value; OnPropertyChanged(); } }
        public static bool IsOpen { get; private set; }


        #region Properties binding;
        private ObservableCollection<TreeElementModel> treeElementsByCategory = new ObservableCollection<TreeElementModel>();
        private ObservableCollection<TreeElementModel> treeElementsByDate = new ObservableCollection<TreeElementModel>();
        private ObservableCollection<TreeElementModel> treeElementsByTypeChange = new ObservableCollection<TreeElementModel>();
        private ObservableCollection<TreeElementModel> treeElementsActive = new ObservableCollection<TreeElementModel>();

        public TrackManagerModel Model { get; set; }
        public ObservableCollection<TreeElementModel> TreeElementsByCategory { get { return treeElementsByCategory; } set { treeElementsByCategory = value; OnPropertyChanged(); } }
        public ObservableCollection<TreeElementModel> TreeElementsByDate { get { return treeElementsByDate; } set { treeElementsByDate = value; OnPropertyChanged(); } }
        public ObservableCollection<TreeElementModel> TreeElementsByTypeChange { get { return treeElementsByTypeChange; } set { treeElementsByTypeChange = value; OnPropertyChanged(); } }
        public ObservableCollection<TreeElementModel> TreeElementsActive { get { return treeElementsActive; } set { treeElementsActive = value; OnPropertyChanged(); } }


        #endregion //Properties biding


        public TrackManagerViewModel(UIApplication uiapp)
        {
            _uiapp = uiapp;
            _doc = _uiapp.ActiveUIDocument.Document;
            Model = new TrackManagerModel();
            try
            {
                TrackManagerWindowLoaded = new RelayCommand(param => this.OnTrackManagerWindowLoaded(param));
                TrackManagerWindowClosed = new RelayCommand(param => this.OnTrackManagerWindowClosed(param));
                ColorSettingsNewElement = new RelayCommand(param => this.OnColorSettingsNewElement(param));
                ColorSettingsChangeFamilyType = new RelayCommand(param => this.OnColorSettingsChangeFamilyType(param));
                ColorSettingsChangeGeometry = new RelayCommand(param => this.OnColorSettingsChangeGeometry(param));
                ColorSettingsChangeRevitParameters = new RelayCommand(param => this.OnColorSettingsChangeRevitParameters(param));
                ColorSettingsChangeSharedParameters = new RelayCommand(param => this.OnColorSettingsChangeSharedParameters(param));
                radBtnByCategory_Checked = new RelayCommand(param => this.OnradBtnByCategory_Checked(param));
                radBtnByTime_Checked = new RelayCommand(param => this.OnradBtnByTime_Checked(param));
                radBtnByTypeChange_Checked = new RelayCommand(param => this.OnradBtnByTypeChange_Checked(param));

            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }

        }

        #region Methods for relay command
        private void OnTrackManagerWindowLoaded(object param)
        {
            IsLoaded = true;
            IsOpen = true;


            //Get Color Revit form color defaut of wpf
            var cNewElement = ((SolidColorBrush)backgroundNewElement).Color;
            var cChangeFamily = ((SolidColorBrush)backgroundFamilyType).Color;
            var cChangeGeo = ((SolidColorBrush)backgroundGeometry).Color;
            var cChangeRvtPara = ((SolidColorBrush)backgroundRevitParameters).Color;
            var cChangeSharedPara = ((SolidColorBrush)backgroundSharedParameters).Color;
            RvtColorNewElement = new Autodesk.Revit.DB.Color(cNewElement.R, cNewElement.G, cNewElement.B);
            RvtColorChangeFamilyType = new Autodesk.Revit.DB.Color(cNewElement.R, cNewElement.G, cNewElement.B);
            RvtColorChangeGeometry = new Autodesk.Revit.DB.Color(cNewElement.R, cNewElement.G, cNewElement.B);
            RvtColorChangeRevitParameters = new Autodesk.Revit.DB.Color(cNewElement.R, cNewElement.G, cNewElement.B);
            RvtColorChangeSharedParameters = new Autodesk.Revit.DB.Color(cNewElement.R, cNewElement.G, cNewElement.B);
        }
        private void OnTrackManagerWindowClosed(object param)
        {
            IsOpen = false;
        }


        private void OnHighLighElement(object param)
        {
            Model.HighLighElement();
        }
        private void OnColorElement(object param)
        {
            Model.ColorElement();
        }
        private void OnRemoveColor(object param)
        {
            Model.RemoveColor();
        }
        private void OnIsolateElement(object param)
        {
            Model.IsolateElement();
        }
        private void OnColorSettingsNewElement(object param)
        {
            //Model.ColorSettings();
            ColorSelectionDialog colorSelectionDialog = new ColorSelectionDialog();
            colorSelectionDialog.Show();
            RvtColorNewElement = colorSelectionDialog.SelectedColor;
            BackgroundNewElement = RevitUtils.ConvertColorRevitToWPF(RvtColorNewElement);

        }
        private void OnColorSettingsChangeFamilyType(object param)
        {
            //Model.ColorSettings();
            ColorSelectionDialog colorSelectionDialog = new ColorSelectionDialog();
            colorSelectionDialog.Show();
            RvtColorChangeFamilyType = colorSelectionDialog.SelectedColor;
            BackgroundFamilyType = RevitUtils.ConvertColorRevitToWPF(RvtColorChangeFamilyType);
        }
        private void OnColorSettingsChangeGeometry(object param)
        {
            //Model.ColorSettings();
            ColorSelectionDialog colorSelectionDialog = new ColorSelectionDialog();
            colorSelectionDialog.Show();
            RvtColorChangeGeometry = colorSelectionDialog.SelectedColor;
            BackgroundGeometry = RevitUtils.ConvertColorRevitToWPF(RvtColorChangeGeometry);
        }
        private void OnColorSettingsChangeRevitParameters(object param)
        {
            //Model.ColorSettings();
            ColorSelectionDialog colorSelectionDialog = new ColorSelectionDialog();
            colorSelectionDialog.Show();
            RvtColorChangeRevitParameters = colorSelectionDialog.SelectedColor;
            BackgroundRevitParameters = RevitUtils.ConvertColorRevitToWPF(RvtColorChangeRevitParameters);
        }
        private void OnColorSettingsChangeSharedParameters(object param)
        {
            //Model.ColorSettings();
            ColorSelectionDialog colorSelectionDialog = new ColorSelectionDialog();
            colorSelectionDialog.Show();
            RvtColorChangeSharedParameters = colorSelectionDialog.SelectedColor;
            BackgroundSharedParameters = RevitUtils.ConvertColorRevitToWPF(RvtColorChangeSharedParameters);
        }

        private void OnradBtnByCategory_Checked(object param)
        {
            TreeElementsActive = new ObservableCollection<TreeElementModel>(treeElementsByCategory);
        }
        private void OnradBtnByTime_Checked(object param)
        {
            TreeElementsActive = new ObservableCollection<TreeElementModel>(treeElementsByDate);
        }
        private void OnradBtnByTypeChange_Checked(object param)
        {
            TreeElementsActive = new ObservableCollection<TreeElementModel>(treeElementsByTypeChange);
        }
        #endregion //Command

        #region Public methods
        public bool DisplayUI()
        {
            bool result = false;
            try
            {
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
        private void CollectElement()
        {
            try
            {
                var catIdList = CategoryUtils.ListCategoryDefaut(_doc).Select(x => x.Id).ToList();
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
                        else if (string.IsNullOrWhiteSpace(change.DateModified) && !string.IsNullOrWhiteSpace(change.ChangeType))
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

        #region Rendre TreeView
        
        #endregion
    }
}
