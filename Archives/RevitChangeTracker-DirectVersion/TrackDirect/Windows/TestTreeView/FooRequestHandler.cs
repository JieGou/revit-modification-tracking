#region References
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using TrackDirect.Models;

#endregion

namespace TrackDirect.UI
{
    public class FooRequestHandler : IExternalEventHandler
    {
        public CommunicatorRequest Request { get; set; } = new CommunicatorRequest();

        #region Input settings for the external events
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
                    case RequestId.GetCategoryList:
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
                    case RequestId.GetElementItems:
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
            GetCategoryList,
            ColorElement,
            UnColorElement,
            GetElementItems
        }

        #endregion //Window Test Resquest
        #endregion // Input settings for the external events

       




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
            IList<ElementId> eleIds = ElementUtils.GetElementInstanceInProject(doc).Select(x => x.Id).ToList();
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

            var eleIds = ElementUtils.GetElementInstanceInProject(doc, false).Select(x => x.Id);
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


        public static ObservableCollection<RevitTreeItem> GetElementItems(UIApplication uiapp)
        {
            ObservableCollection<RevitTreeItem> TreeItems = new ObservableCollection<RevitTreeItem>();
            Application app = uiapp.Application;
            Document doc = uiapp.ActiveUIDocument.Document;
            IList<Element> eles = ElementUtils.GetElementInstanceInProject(doc, false);
            
            ObservableCollection<RevitElement> rvtElements = new ObservableCollection<RevitElement>();
            foreach (Element e in eles)
            {
                
              
            }

            return TreeItems;
        }

        public static void AddItemIntoTreeViewItem(ObservableCollection<RevitTreeItem> root, RevitTreeItem node)
        {
            root.Add(node);
        }

        #endregion //TreeView Category, family list, element

        #endregion //External events



    }
}