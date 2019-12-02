#region References
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Interop;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using GalaSoft.MvvmLight.Messaging;

#endregion

namespace TrackChanges
{
    public class WindowTestRequestHandler : IExternalEventHandler
    {
        public CommunicatorRequest Request { get; set; } = new CommunicatorRequest();


        public string GetName()
        {
            return "Task External Event";
        }

        public void Execute(UIApplication app)
        {
            try
            {
                switch (Request.Take())
                {
                    case RequestId.None:
                        {
                            return;
                        }
                    case RequestId.SelectElement:
                        {
                            SelectElement(app);
                            break;
                        }
                    case RequestId.ColorElement:
                        {
                            ColorElement(app);
                            break;
                        }
                    case RequestId.UnColorElement:
                        {
                            UnColorElement(app);
                            break;
                        }
                    case RequestId.GetTreeView:
                    {
                        UnColorElement(app);
                        break;
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        #region WindowTestRequest

        public class CommunicatorRequest
        {
            private int _request = (int)RequestId.None;

            public RequestId Take()
            {
                return (RequestId)Interlocked.Exchange(ref _request, (int)RequestId.None);
            }

            public void Make(RequestId request)
            {
                Interlocked.Exchange(ref _request, (int)request);
            }
        }

        public enum RequestId
        {
            None,
            SelectElement,
            ColorElement,
            UnColorElement, 
            GetTreeView
        }

        #endregion



        #region External event

        /// <summary>
        /// Opens selected View Sheet.
        /// </summary>
        /// <param name="app">UI App.</param>
        private void SelectElement(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            IList<ElementId> eleIds = ElementUtils.GetElementInProject(doc).Select(x => x.Id).ToList();
            uidoc.Selection.SetElementIds(eleIds);
            uidoc.ShowElements(eleIds);
        }

        public void ColorElement(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;
            CategorySet categories = CategoryUtils.GetCategoriesHasElements(doc, app);
            IList<ElementId> eleIds = ElementUtils.GetElementList(doc, categories).Select(x => x.Id).ToList();



            uidoc.Selection.SetElementIds(eleIds);
            try
            {
                Color color = new Color(255, 0, 0);
                OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                var patternCollector = new FilteredElementCollector(doc.ActiveView.Document);
                patternCollector.OfClass(typeof(Autodesk.Revit.DB.FillPatternElement));
                FillPatternElement solidFill = patternCollector
                    .ToElements()
                    .Cast<Autodesk.Revit.DB.FillPatternElement>()
                    .First(x => x.GetFillPattern().IsSolidFill);


                ogs.SetProjectionLineColor(color); // or other here
                ogs.SetHalftone(true);
                ogs.SetProjectionLineColor(color);
#pragma warning disable CS0618 // Type or member is obsolete
                ogs.SetProjectionFillPatternId(solidFill.Id);
#pragma warning restore CS0618 // Type or member is obsolete
                using (Transaction t = new Transaction(doc, "Set Element Override"))
                {
                    t.Start();
                    foreach (var id in eleIds)
                    {
                        doc.ActiveView.SetElementOverrides(id, ogs);
                    }

                    t.Commit();
                }
            }
            catch
            {

                throw;
            }

            uidoc.ShowElements(eleIds);
        }

        public void UnColorElement(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;
            CategorySet categories = CategoryUtils.GetCategoriesHasElements(doc, app);
            IList<ElementId> eleIds = ElementUtils.GetElementList(doc, categories).Select(x => x.Id).ToList();



            uidoc.Selection.SetElementIds(eleIds);
            try
            {
                OverrideGraphicSettings ogs = new OverrideGraphicSettings();

                using (Transaction t = new Transaction(doc, "Set Element Override"))
                {
                    t.Start();
                    foreach (var id in eleIds)
                    {
                        doc.ActiveView.SetElementOverrides(id, ogs);
                    }

                    t.Commit();
                }
            }
            catch
            {
                throw;
            }
            uidoc.ShowElements(eleIds);
        }

        public ObservableCollection<ElementId> GetElementIdList(UIApplication uiapp)
        {
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uiapp.ActiveUIDocument.Document;

            var eleIds = ElementUtils.GetElementInProject(doc, false).Select(x => x.Id);
            return new ObservableCollection<ElementId>(eleIds);

        }

        public ObservableCollection<string> GetAllView(UIApplication uiapp)
        {
            // viewtypename
            Document doc = uiapp.ActiveUIDocument.Document;
            ObservableCollection<string> viewTypenames = new ObservableCollection<string>();

            // collect view type
            List<Element> elements = new FilteredElementCollector(doc).OfClass(typeof(View)).ToList();

            foreach (Element element in elements)
            {
                // view
                View view = element as View;
                // view typename
                viewTypenames.Add(view.ViewType.ToString());
            }

            return viewTypenames;
        }


        #region TreeView Category, family list, element


        public ObservableCollection<MenuTreeItem> GetListTreeElement(UIApplication uiapp)
        {
            ObservableCollection<MenuTreeItem> TreeItems = new ObservableCollection<MenuTreeItem>();
            Application app = uiapp.Application;
            Document doc = uiapp.ActiveUIDocument.Document;
            var catSet = CategoryUtils.GetCategoriesHasElements(doc, app);
            ObservableCollection<Category> catList = new ObservableCollection<Category>();
            foreach (Category cat in catSet)
            {
                catList.Add(cat);
                MenuTreeItem item = new MenuTreeItem();
                item.Name = cat.ToString();
                AddItemIntoTreeViewItem(TreeItems, item);
                ObservableCollection<Family> lstFamily = new ObservableCollection<Family>(
                    new FilteredElementCollector(doc)
                        .OfClass(typeof(Family))
                        .Cast<Family>());

                foreach (var f in lstFamily)
                {
                    MenuTreeItem familyItem = new MenuTreeItem();
                    familyItem.Name = familyItem.Name.ToString();
                    AddItemIntoTreeViewItem(item.Items, familyItem);

                }
            }

            return TreeItems;
        }

        public void AddItemIntoTreeViewItem(ObservableCollection<MenuTreeItem> root, MenuTreeItem node)
        {
            root.Add(node);
        }

        #endregion


        #endregion



    }
}