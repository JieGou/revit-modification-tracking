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

    public enum SortRoot
    {
        Category,
        Date,
        TypeChange
    }
    public class TreeElementModel : INotifyPropertyChanged
    {

        private bool? isChecked = false;
        public string Name { get; set; }
        public object Tag { get; set; }
        public bool IsInitiallySelected { get; set; }
        public bool IsExpanded { get; set; }
        public string ToolTip { get; set; } = string.Empty;
        public Visibility ToolTipVisibility { get; set; }
        public TreeViewNodeType NodeType { get; set; }

        public TreeElementModel ParentNode { get; set; }
        public ObservableCollection<TreeElementModel> ChildrenNodes { get; set; }


        public TreeElementModel()
        {
            ChildrenNodes = new ObservableCollection<TreeElementModel>();
        }
        public TreeElementModel(string nodeName)
        {
            Name = nodeName;
            Tag = null;
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

        public static ObservableCollection<TreeElementModel> RenderByCategory()
        {
            ObservableCollection<TreeElementModel> root = new ObservableCollection<TreeElementModel>();

            try
            {
                root.Add(GetRootNewElements(SortRoot.Category));
                root.Add(GetRootModifiedElements(SortRoot.Category));
                root.Add(GetRootUnclassifiedElement());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to set the tree view.\n" + ex.Message, "Set Tree View", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return root;
        }
        public static ObservableCollection<TreeElementModel> RenderByDate()
        {
            ObservableCollection<TreeElementModel> root = new ObservableCollection<TreeElementModel>();

            try
            {
                root.Add(GetRootNewElements(SortRoot.Date));
                root.Add(GetRootModifiedElements(SortRoot.Date));
                root.Add(GetRootUnclassifiedElement());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to set the tree view.\n" + ex.Message, "Set Tree View", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return root;
        }

        public static ObservableCollection<TreeElementModel> RenderByTypeChange()
        {
            ObservableCollection<TreeElementModel> root = new ObservableCollection<TreeElementModel>();

            try
            {
                root.Add(GetRootNewElements(SortRoot.Category));
                root.Add(GetRootModifiedElements(SortRoot.TypeChange));
                root.Add(GetRootUnclassifiedElement());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to set the tree view.\n" + ex.Message, "Set Tree View", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return root;
        }

        private static TreeElementModel GetRootNewElements(SortRoot sortBy)
        {
            TreeElementModel rootNewElements = new TreeElementModel($"New Elements ({ChangeComponentFilter.NewElements.Count })");
            rootNewElements.IsInitiallySelected = true;
            rootNewElements.ToolTipVisibility = Visibility.Collapsed;
            rootNewElements.NodeType = TreeViewNodeType.Root;

            switch (sortBy)
            {
                case SortRoot.Category:
                    {
                        foreach (var cat in ChangeComponentFilter.NewElements.GroupBy(c => c.CategoryName).ToDictionary(c => c.Key, c => c.ToList()))
                        {
                            var catNode = new TreeElementModel(cat.Key);
                            catNode.Tag = "Category";
                            catNode.NodeType = TreeViewNodeType.Category;
                            catNode.ToolTipVisibility = Visibility.Collapsed;
                            catNode.IsExpanded = true;
                            rootNewElements.ChildrenNodes.Add(catNode);

                            foreach (var itemDate in cat.Value.GroupBy(c => c.DateCreated).ToDictionary(c => c.Key, c => c.ToList()))
                            {

                                var itemDateNode = new TreeElementModel(itemDate.Key);
                                itemDateNode.Tag = "Date";
                                itemDateNode.NodeType = TreeViewNodeType.Date;
                                itemDateNode.ToolTipVisibility = Visibility.Collapsed;
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

                        }
                        rootNewElements.Initialize();
                        return rootNewElements;
                    }

                case SortRoot.Date:
                    {
                        foreach (var itemDate in ChangeComponentFilter.NewElements.GroupBy(c => c.DateCreated).ToDictionary(c => c.Key, c => c.ToList()))
                        {
                            var itemDateNode = new TreeElementModel(itemDate.Key);
                            itemDateNode.Tag = "Date";
                            itemDateNode.NodeType = TreeViewNodeType.Date;
                            itemDateNode.ToolTipVisibility = Visibility.Collapsed;
                            itemDateNode.IsExpanded = true;
                            rootNewElements.ChildrenNodes.Add(itemDateNode);

                            foreach (var cat in itemDate.Value.GroupBy(c => c.CategoryName).ToDictionary(c => c.Key, c => c.ToList()))
                            {
                                var catNode = new TreeElementModel(cat.Key);
                                catNode.Tag = "Category";
                                catNode.NodeType = TreeViewNodeType.Category;
                                catNode.ToolTipVisibility = Visibility.Collapsed;
                                itemDateNode.ChildrenNodes.Add(catNode);


                                foreach (var com in cat.Value)
                                {
                                    var elementNode = new TreeElementModel(com.Name);
                                    elementNode.Tag = com as ChangeComponent;
                                    elementNode.NodeType = TreeViewNodeType.Element;
                                    elementNode.ToolTip = com.ChangeType;
                                    elementNode.ToolTipVisibility = Visibility.Visible;

                                    catNode.ChildrenNodes.Add(elementNode);
                                }
                            }
                        }
                        rootNewElements.Initialize();
                        return rootNewElements;
                    }

                default:
                    return rootNewElements;


            }
        }

        private static TreeElementModel GetRootModifiedElements(SortRoot sortBy)
        {
            TreeElementModel rootModifiedElements = new TreeElementModel($"Modified Elements ({ChangeComponentFilter.ModifiedElements.Count })");
            rootModifiedElements.IsInitiallySelected = true;
            rootModifiedElements.ToolTipVisibility = Visibility.Collapsed;
            rootModifiedElements.NodeType = TreeViewNodeType.Root;

            switch (sortBy)
            {
                case SortRoot.Category:
                    {
                        foreach (var cat in ChangeComponentFilter.ModifiedElements.GroupBy(c => c.CategoryName).ToDictionary(c => c.Key, c => c.ToList()))
                        {
                            var catNode = new TreeElementModel(cat.Key);
                            catNode.Tag = "Category";
                            catNode.NodeType = TreeViewNodeType.Category;
                            catNode.ToolTipVisibility = Visibility.Collapsed;
                            catNode.IsExpanded = true;  
                            rootModifiedElements.ChildrenNodes.Add(catNode);

                            foreach (var itemDate in cat.Value.GroupBy(c => c.DateModified).ToDictionary(c => c.Key, c => c.ToList()))
                            {

                                var itemDateNode = new TreeElementModel(itemDate.Key);
                                itemDateNode.Tag = "Date";
                                itemDateNode.NodeType = TreeViewNodeType.Date;
                                itemDateNode.ToolTipVisibility = Visibility.Collapsed;
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

                        }
                        rootModifiedElements.Initialize();
                        return rootModifiedElements;
                    }

                case SortRoot.Date:
                    {
                        foreach (var itemDate in ChangeComponentFilter.ModifiedElements.GroupBy(c => c.DateModified).ToDictionary(c => c.Key, c => c.ToList()))
                        {
                            var itemDateNode = new TreeElementModel(itemDate.Key);
                            itemDateNode.Tag = "Date";
                            itemDateNode.NodeType = TreeViewNodeType.Date;
                            itemDateNode.ToolTipVisibility = Visibility.Collapsed;
                            itemDateNode.IsExpanded = true;
                            rootModifiedElements.ChildrenNodes.Add(itemDateNode);

                            foreach (var cat in itemDate.Value.GroupBy(c => c.CategoryName).ToDictionary(c => c.Key, c => c.ToList()))
                            {
                                var catNode = new TreeElementModel(cat.Key);
                                catNode.Tag = "Category";
                                catNode.NodeType = TreeViewNodeType.Category;
                                catNode.ToolTipVisibility = Visibility.Collapsed;
                                itemDateNode.ChildrenNodes.Add(catNode);


                                foreach (var com in cat.Value)
                                {
                                    var elementNode = new TreeElementModel(com.Name);
                                    elementNode.Tag = com as ChangeComponent;
                                    elementNode.NodeType = TreeViewNodeType.Element;
                                    elementNode.ToolTip = com.ChangeType;
                                    elementNode.ToolTipVisibility = Visibility.Visible;

                                    catNode.ChildrenNodes.Add(elementNode);
                                }
                            }
                        }
                        rootModifiedElements.Initialize();
                        return rootModifiedElements;
                    }

                case SortRoot.TypeChange:
                    {
                        foreach (var itemTypeChange in ChangeComponentFilter.ModifiedElements.GroupBy(c => c.ChangeType).ToDictionary(c => c.Key, c => c.ToList()))
                        {
                            var itemTypeChangeNode = new TreeElementModel(itemTypeChange.Key);
                            itemTypeChangeNode.Tag = "TypeChange";
                            itemTypeChangeNode.NodeType = TreeViewNodeType.Date;
                            itemTypeChangeNode.ToolTipVisibility = Visibility.Collapsed;
                            itemTypeChangeNode.IsExpanded = true;   
                            rootModifiedElements.ChildrenNodes.Add(itemTypeChangeNode);

                            foreach (var cat in itemTypeChange.Value.GroupBy(c => c.CategoryName).ToDictionary(c => c.Key, c => c.ToList()))
                            {
                                var catNode = new TreeElementModel(cat.Key);
                                catNode.Tag = "Category";
                                catNode.NodeType = TreeViewNodeType.Category;
                                catNode.ToolTipVisibility = Visibility.Collapsed;
                                itemTypeChangeNode.ChildrenNodes.Add(catNode);


                                foreach (var com in cat.Value)
                                {
                                    var elementNode = new TreeElementModel(com.Name);
                                    elementNode.Tag = com as ChangeComponent;
                                    elementNode.NodeType = TreeViewNodeType.Element;
                                    elementNode.ToolTip = com.ChangeType;
                                    elementNode.ToolTipVisibility = Visibility.Visible;

                                    catNode.ChildrenNodes.Add(elementNode);
                                }
                            }
                        }
                        rootModifiedElements.Initialize();
                        return rootModifiedElements;

                    }

                default:
                    return rootModifiedElements;

            }
            
        }

        private static TreeElementModel GetRootUnclassifiedElement()
        {
            var rootUnClassifiedElement = new TreeElementModel($"Unclassified Elements ({ChangeComponentFilter.UnClassifiedElements.Count })");
            rootUnClassifiedElement.IsInitiallySelected = true;
            rootUnClassifiedElement.ToolTipVisibility = Visibility.Collapsed;
            rootUnClassifiedElement.NodeType = TreeViewNodeType.Root;


            foreach (var cat in ChangeComponentFilter.UnClassifiedElements.GroupBy(c => c.CategoryName).ToDictionary(c => c.Key, c => c.ToList()))
            {
                var catNode = new TreeElementModel(cat.Key);
                catNode.Tag = "Category";
                catNode.NodeType = TreeViewNodeType.Category;
                catNode.ToolTipVisibility = Visibility.Collapsed;
                catNode.IsExpanded = true;
                rootUnClassifiedElement.ChildrenNodes.Add(catNode);
                foreach (var com in cat.Value)
                {
                    var elementNode = new TreeElementModel(com.Name);
                    elementNode.Tag = com as ChangeComponent;
                    elementNode.NodeType = TreeViewNodeType.Element;
                    elementNode.ToolTipVisibility = Visibility.Collapsed;

                    catNode.ChildrenNodes.Add(elementNode);
                }

            }
            rootUnClassifiedElement.Initialize();
            return rootUnClassifiedElement;
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
