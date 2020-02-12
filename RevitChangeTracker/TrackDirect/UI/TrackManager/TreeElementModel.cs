using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using TrackDirect.Models;
using Visibility = System.Windows.Visibility;

namespace TrackDirect.UI
{
    public enum TreeViewNodeType
    {
        None,
        Root,
        TypeTrack,
        Date,
        Category,
        Element
    }

    public class TreeElementModel : INotifyPropertyChanged
    {

        private bool? isChecked = false;

        public string Name { get; private set; }
        public ElementId Tag { get; private set; }
        public bool IsInitiallySelected { get; private set; }
        public TreeElementModel ParentNode { get; private set; }
        public List<TreeElementModel> ChildrenNodes { get; private set; }
        public TreeViewNodeType NodeType { get; private set; }
        public string ToolTip { get; private set; }
        public Visibility ToolTipVisibility { get; private set; }
        public string ChangedType { get; private set; }
        public DateTime DateCreated { get; private set; }
        public DateTime DateModifyed { get; private set; }

        public TreeElementModel(string nodeName)
        {
            Name = nodeName;
            Tag = null;
            ChildrenNodes = new List<TreeElementModel>();
            ToolTip = "";
        }

        void Initialize()
        {
            foreach (var child in ChildrenNodes)
            {
                child.ParentNode = this;
                child.Initialize();
            }
        }

        public bool? IsChecked
        {
            get { return isChecked; }
            set { SetIsChecked(value, true, true); }
        }

        public void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == isChecked) return;
            isChecked = value;
            if (updateChildren && isChecked.HasValue) { ChildrenNodes.ForEach(c => c.SetIsChecked(isChecked, true, false)); }
            if (updateParent && ParentNode != null) { ParentNode.VerifyCheckedState(); }
            OnPropertyChanged("IsChecked");
        }

        public void VerifyCheckedState()
        {
            bool? state = null;

            for (var i = 0; i < ChildrenNodes.Count; ++i)
            {
                var current = ChildrenNodes[i].IsChecked;
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

            SetIsChecked(state, false, true);
        }

        public static ObservableCollection<TreeElementModel> RenderTreeViewByCategory()
        {
            var treeView = new ObservableCollection<TreeElementModel>();
            var uiapp = CmdTrackManager.Uiapp;
            Document doc = uiapp.ActiveUIDocument.Document;
            try
            {
                List<Category> customCategories = CategoryUtils.ListCategoryDefaut(doc);
                foreach (var cat in customCategories)
                {
                    var categoryNode = new TreeElementModel(cat.Name);
                    categoryNode.NodeType = TreeViewNodeType.Category;
                    categoryNode.ToolTipVisibility = Visibility.Hidden;
                    treeView.Add(categoryNode);

                    var elements = ElementUtils.GetElementsByCategories(doc,cat.Id);
                        foreach (var ele in elements)
                        {
                        string fullName = "GetElementFullName(ele)";
                            var elementNode = new TreeElementModel(fullName);
                            elementNode.Tag = ele.Id;
                            elementNode.NodeType = TreeViewNodeType.Element;
                            elementNode.ToolTip = "ChangeType";
                            elementNode.ToolTipVisibility = Visibility.Visible;
                            categoryNode.ChildrenNodes.Add(elementNode);
                        }

                    categoryNode.Initialize();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to set the tree view.\n" + ex.Message, "Set Tree View", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return treeView;
        }

        #region Event property change
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

}
