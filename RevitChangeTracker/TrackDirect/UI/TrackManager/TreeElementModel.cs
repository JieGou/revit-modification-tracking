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
        Root,
        TypeTrack,
        Category,
        Date,
        Element
    }
    public class TreeElementModel : INotifyPropertyChanged
    {

        private bool? isChecked = false;


        public string Name { get; private set; }
        public object Tag { get; private set; }
        public bool IsInitiallySelected { get; private set; }
        public string ToolTip { get; private set; }
        public Visibility ToolTipVisibility { get; private set; }
        public TreeViewNodeType NodeType { get; private set; }

        public TreeElementModel ParentNode { get; private set; }
        public ObservableCollection<TreeElementModel> ChildrenNodes { get; private set; }


        public TreeElementModel()
        {
            ChildrenNodes = new ObservableCollection<TreeElementModel>();
        }
        public TreeElementModel(string nodeName)
        {
            Name = nodeName;
            ChildrenNodes = new ObservableCollection<TreeElementModel>();
        }


        #region Selection TreeviewItem
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
            if (updateChildren && isChecked.HasValue) { ChildrenNodes.ToList().ForEach(c => c.SetIsChecked(isChecked, true, false)); }
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
        #endregion

        #region Rendre TreeView
        public static ObservableCollection<TreeElementModel> RenderByCategory()
        {
            var root = new ObservableCollection<TreeElementModel>();
            try
            {
                var rootNewElements = new TreeElementModel($"New Elements ({ChangeComponentFilter.NewElements.Count })");
                rootNewElements.IsInitiallySelected = true;
                rootNewElements.ToolTipVisibility = Visibility.Hidden;
                rootNewElements.NodeType = TreeViewNodeType.Root;
                root.Add(rootNewElements);
                foreach (var group in ChangeComponentFilter.NewElements.GroupBy(c => c.CategoryName).ToDictionary(c => c.Key, c => c.ToList()))
                {
                    var catNode = new TreeElementModel(group.Key);
                    catNode.Tag = "Category";
                    catNode.NodeType = TreeViewNodeType.Category;
                    catNode.ToolTipVisibility = Visibility.Hidden;
                    rootNewElements.ChildrenNodes.Add(catNode);
                    foreach (var itemDate in group.Value.GroupBy(c => c.DateCreated).ToDictionary(c => c.Key, c => c.ToList()))
                    {

                        var itemDateNode = new TreeElementModel(itemDate.Key);
                        itemDateNode.Tag = "Date";
                        itemDateNode.NodeType = TreeViewNodeType.Date;
                        itemDateNode.ToolTipVisibility = Visibility.Hidden;

                        catNode.ChildrenNodes.Add(itemDateNode);
                        foreach (var com in itemDate.Value)
                        {
                            var elementNode = new TreeElementModel(com.Name);
                            elementNode.Tag = com as ChangeComponent;
                            elementNode.NodeType = TreeViewNodeType.Element;
                            elementNode.ToolTip = com.ChangeType;
                            elementNode.ToolTipVisibility = Visibility.Visible;

                            itemDateNode.ChildrenNodes.Add(elementNode);
                        }
                    }
                    catNode.Initialize();

                }
                rootNewElements.Initialize();


                var rootModifiedElements = new TreeElementModel($"Modified Elements ({ChangeComponentFilter.ModifiedElements.Count })");
                rootModifiedElements.IsInitiallySelected = true;
                rootModifiedElements.ToolTipVisibility = Visibility.Hidden;
                rootModifiedElements.NodeType = TreeViewNodeType.Root;
                root.Add(rootModifiedElements);

                foreach (var group in ChangeComponentFilter.ModifiedElements.GroupBy(c => c.CategoryName).ToDictionary(c => c.Key, c => c.ToList()))
                {
                    var catNode = new TreeElementModel(group.Key);
                    catNode.Tag = "Category";
                    catNode.NodeType = TreeViewNodeType.Category;
                    catNode.ToolTipVisibility = Visibility.Hidden;
                    rootModifiedElements.ChildrenNodes.Add(catNode);

                    foreach (var itemDate in group.Value.GroupBy(c => c.DateCreated).ToDictionary(c => c.Key, c => c.ToList()))
                    {

                        var itemDateNode = new TreeElementModel(itemDate.Key);
                        itemDateNode.Tag = "Date";
                        itemDateNode.NodeType = TreeViewNodeType.Date;
                        itemDateNode.ToolTipVisibility = Visibility.Hidden;
                        catNode.ChildrenNodes.Add(itemDateNode);
                        foreach (var com in itemDate.Value)
                        {
                            var elementNode = new TreeElementModel(com.Name);
                            elementNode.Tag = com as ChangeComponent;
                            elementNode.NodeType = TreeViewNodeType.Element;
                            elementNode.ToolTip = com.ChangeType;
                            elementNode.ToolTipVisibility = Visibility.Visible;

                            itemDateNode.ChildrenNodes.Add(elementNode);
                        }
                    }
                    catNode.Initialize();
                }

                rootModifiedElements.Initialize();

                var rootUnClassifiedElement = new TreeElementModel($"Unclassified Elements ({ChangeComponentFilter.UnClassifiedElements.Count })");
                rootUnClassifiedElement.IsInitiallySelected = true;
                rootUnClassifiedElement.ToolTipVisibility = Visibility.Hidden;
                rootUnClassifiedElement.NodeType = TreeViewNodeType.Root;
                root.Add(rootUnClassifiedElement);

                foreach (var group in ChangeComponentFilter.UnClassifiedElements.GroupBy(c => c.CategoryName).ToDictionary(c => c.Key, c => c.ToList()))
                {
                    var catNode = new TreeElementModel(group.Key);
                    catNode.Tag = "Category";
                    catNode.NodeType = TreeViewNodeType.Category;
                    catNode.ToolTipVisibility = Visibility.Hidden;
                    rootModifiedElements.ChildrenNodes.Add(catNode);
                    foreach (var com in group.Value)
                    {
                        var elementNode = new TreeElementModel(com.Name);
                        elementNode.Tag = com as ChangeComponent;
                        elementNode.NodeType = TreeViewNodeType.Element;
                        elementNode.ToolTip = com.ChangeType;
                        elementNode.ToolTipVisibility = Visibility.Visible;

                        catNode.ChildrenNodes.Add(elementNode);
                    }
                    catNode.Initialize();
                }
                rootUnClassifiedElement.Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to set the tree view.\n" + ex.Message, "Set Tree View", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
           
           
            return root;
        }
        #endregion



        #region Event property change
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

}
