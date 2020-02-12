using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TrackDirect.Models;

namespace TrackDirect.UI
{
    public class TrackManagerViewModel : BaseViewModel
    {
        #region RelayCommand
        public RelayCommand TrackManagerWindowLoaded { get; set; }
        #endregion //Relay command


        private UIApplication _uiapp = null;
        private Document _doc = null;
     
        public Document Doc { get { return _doc; } set { _doc = value; } }
        public bool IsLoaded = false;

        #region Properties binding;
        private ObservableCollection<TreeElementModel> treeElements= new ObservableCollection<TreeElementModel>();

        public TrackManagerModel Model { get; set; }
        public ObservableCollection<TreeElementModel> TreeElements { get { return treeElements; } set { treeElements = value; OnPropertyChanged(); } }
        #endregion //Properties biding


        public TrackManagerViewModel(UIApplication uiapp)
        {
            _uiapp = uiapp;
            _doc = _uiapp.ActiveUIDocument.Document;
            Model = new TrackManagerModel();
            try
            {
                TrackManagerWindowLoaded = new RelayCommand(param => this.OnTrackManagerWindowLoaded(param));
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
            //TreeElements = TreeElementModel.RenderTreeViewByCategory();
        }

        private void OnHighLighElement()
        {
            Model.HighLighElement();
        }
        public void OnColorElement()
        {
            Model.ColorElement();
        }
        public void OnRemoveColor()
        {
            Model.RemoveColor();
        }
        public void OnIsolateElement()
        {
            Model.IsolateElement();
        }

        #endregion //Command

        #region Public methods
        public  bool DisplayUI()
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
                        } else
                        {
                            ChangeComponentFilter.UnClassifiedElements.Add(change);
                        }
                    }
                }
                treeElements = TreeElementModel.RenderByCategory();
  
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to Collect Element.\n" + ex.Message, "TrackManager: CollectElement", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion


    }
}
