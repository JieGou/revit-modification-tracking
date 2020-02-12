using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

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
        private ObservableCollection<TreeViewElementModel> treeElements= new ObservableCollection<TreeViewElementModel>();


        public ObservableCollection<TreeViewElementModel> TreeElements { get { return treeElements; } set { treeElements = value; OnPropertyChanged(); } }
        #endregion //Properties biding


        public TrackManagerViewModel(UIApplication uiapp)
        {
            _uiapp = uiapp;
            _doc = _uiapp.ActiveUIDocument.Document;
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
            TreeElements = TreeViewElementModel.RenderTreeViewByCategory();
        }


        #endregion //Command



    }
}
