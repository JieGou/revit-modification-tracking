#region References
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Drawing;
using TrackDirect.Models;
using System.Windows.Media;
using TrackDirect.Utilities;

#endregion

namespace TrackDirect.UI
{
    public class TrackManagerHandler : IExternalEventHandler
    {
        private static Document _doc;
        private static UIApplication _uiapp;
        private static Autodesk.Revit.ApplicationServices.Application _app;
        private static UIDocument _uidoc;
        private TrackManagerViewModel viewModel = null;

        public TrackManagerViewModel ViewModel { get { return viewModel; } set { viewModel = value; } }

        public TrackManagerHandler()
        {

        }
        public string GetName()
        {
            return "Task External Event1";
        }
        public void Execute(UIApplication uiapp)
        {
            _uiapp = uiapp;
            _uidoc = uiapp.ActiveUIDocument;
            _app = uiapp.Application;
            _doc = uiapp.ActiveUIDocument.Document;
            try
            {
                switch (Request.Take())
                {
                    case RequestId.None:
                        {
                            return;
                        }
                    case RequestId.ApplyView:
                        {
                            SaveSettingsJson();
                            List<ElementId> elemIds = new List<ElementId>();
                            IList<ElementId> selectedNewElemIds = new List<ElementId>();
                            IList<ElementId> selectedChangeFamilyElemIds = new List<ElementId>();
                            IList<ElementId> selectedChangeGeoElemIds = new List<ElementId>();
                            IList<ElementId> selectedChangeVolumeElemIds = new List<ElementId>();
                            IList<ElementId> selectedChangeRvtParaElemIds = new List<ElementId>();
                            IList<ElementId> selectedChangeSharedParaElemIds = new List<ElementId>();
                            try
                            {
                                var componentsNew = viewModel.TreeElementsActive.Where(x => x.NodeType == TreeViewNodeType.Element && x.IsChecked == true).Select(x => (ChangeComponent)x.Tag).ToList();
                                var root = viewModel.TreeElementsActive.Where(x => x.NodeType == TreeViewNodeType.Root).ToList();
                                //Get root treeview
                                var rootNewElems = Flatten(root.FirstOrDefault());
                                var rootModifElems = Flatten(root[1]);
                                var rootUnClassElems = Flatten(root.LastOrDefault());

                                //Get check changeComponent

                                IList<ChangeComponent> selectedNewComponent = rootNewElems.Where(x => x.NodeType == TreeViewNodeType.Element && x.IsChecked == true).Select(x => (ChangeComponent)x.Tag).ToList();
                                IList<ChangeComponent> selectedModifComponent = rootModifElems.Where(x => x.NodeType == TreeViewNodeType.Element && x.IsChecked == true).Select(x => (ChangeComponent)x.Tag).ToList();
                                IList<ChangeComponent> selectedUnClassComponent = rootUnClassElems.Where(x => x.NodeType == TreeViewNodeType.Element && x.IsChecked == true).Select(x => (ChangeComponent)x.Tag).ToList();

                                //Get modified compoments detail change type
                                string fa = ChangedElement.ChangeTypeEnum.FamilyOrTypeChange.ToString();
                                string geo = ChangedElement.ChangeTypeEnum.GeometryChange.ToString();
                                string vol = ChangedElement.ChangeTypeEnum.VolumeOrLocationChange.ToString();
                                string rvtP = ChangedElement.ChangeTypeEnum.RevitParameterChange.ToString();
                                string sharedP = ChangedElement.ChangeTypeEnum.SharedParameterChange.ToString();

                                IList<ChangeComponent> changeFamilyCompo = selectedModifComponent.Where(x => x.ChangeType == fa).ToList();
                                IList<ChangeComponent> changeGeoCompo = selectedModifComponent.Where(x => x.ChangeType == geo).ToList();
                                IList<ChangeComponent> changeVolumeCompo = selectedModifComponent.Where(x => x.ChangeType == vol).ToList();
                                IList<ChangeComponent> changeRvtPCompo = selectedModifComponent.Where(x => x.ChangeType == rvtP).ToList();
                                IList<ChangeComponent> changeSharedPCompo = selectedModifComponent.Where(x => x.ChangeType == sharedP).ToList();

                                //Get ElementId from Changed Component
                                selectedNewElemIds = selectedNewComponent.Select(x => _doc.GetElement(x.Id)).Select(x => x.Id).ToList();
                                IEnumerable<ElementId> selectedModifElemIds = selectedModifComponent.Select(x => _doc.GetElement(x.Id)).Select(x => x.Id);
                                IEnumerable<ElementId> selectedUnClassElemIds = selectedUnClassComponent.Select(x => _doc.GetElement(x.Id)).Select(x => x.Id);

                                //GEt all list selected element
                                elemIds.AddRange(selectedNewElemIds);
                                elemIds.AddRange(selectedModifElemIds);
                                elemIds.AddRange(selectedUnClassElemIds);

                                //Get elements id with change type
                                selectedChangeFamilyElemIds = changeFamilyCompo.Select(x => _doc.GetElement(x.Id)).Select(x => x.Id).ToList();
                                selectedChangeGeoElemIds = changeGeoCompo.Select(x => _doc.GetElement(x.Id)).Select(x => x.Id).ToList();
                                selectedChangeVolumeElemIds = changeVolumeCompo.Select(x => _doc.GetElement(x.Id)).Select(x => x.Id).ToList();
                                selectedChangeRvtParaElemIds = changeRvtPCompo.Select(x => _doc.GetElement(x.Id)).Select(x => x.Id).ToList();
                                selectedChangeSharedParaElemIds = changeSharedPCompo.Select(x => _doc.GetElement(x.Id)).Select(x => x.Id).ToList();

                            }
                            catch { }

                            if (viewModel.IsHighLight)
                            {
                                HighLightElement(elemIds);
                            }
                            if (viewModel.IsIsolate)
                            {
                                IsolateElement(elemIds);
                            }
                            if (viewModel.IsColorElement)
                            {
                                ColorElement(selectedNewElemIds, viewModel.RvtColorNewElement);
                                ColorElement(selectedChangeFamilyElemIds, viewModel.RvtColorChangeFamilyType);
                                ColorElement(selectedChangeGeoElemIds, viewModel.RvtColorChangeGeometry);
                                ColorElement(selectedChangeVolumeElemIds, viewModel.RvtColorChangeVolume);
                                ColorElement(selectedChangeRvtParaElemIds, viewModel.RvtColorChangeRvtPara);
                                ColorElement(selectedChangeSharedParaElemIds, viewModel.RvtColorChangeSharedPara);

                            }
                            break;
                        }

                    case RequestId.RemoveColor:
                        {
                            if (viewModel.SelectOption == ComponentOption.OnlyVisible)
                            {
                                RemoveColorActiveView();

                            }
                            else if (viewModel.SelectOption == ComponentOption.SelectedElements)
                            {
                                RemoveColorSelectedElement();
                            }
                            else if (viewModel.SelectOption == ComponentOption.EntireModel)
                            {
                                RemoveColorEntireModel();
                            }
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
        public CommunicatorRequest Request { get; set; } = new CommunicatorRequest();
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
            ApplyView,
            RemoveColor
        }

        #region External event

        private void HighLightElement(IList<ElementId> elemIds)
        {
            DisableTemperaryMode();
            _uidoc.Selection.SetElementIds(elemIds);
            _uidoc.ShowElements(elemIds);
        }

        private void DisableTemperaryMode()
        {
            try
            {
                View view = _doc.ActiveView;
                using (Transaction tr = new Transaction(_doc, "Unhide"))
                {
                    tr.Start();
                    if (view.IsTemporaryHideIsolateActive())
                    {
                        TemporaryViewMode tempView = TemporaryViewMode.TemporaryHideIsolate;
                        view.DisableTemporaryViewMode(tempView);
                    }
                    tr.Commit();
                }
            }
            catch {}
        }
        private void IsolateElement(IList<ElementId> elemIds)
        {
            try
            {
                var view = _doc.ActiveView;
                using (Transaction t = new Transaction(_doc, "IsolateElement"))
                {
                    t.Start();
                    view.IsolateElementsTemporary(elemIds);
                    t.Commit();
                }
            }
            catch {}
        }

        private void treeByCategory()
        {

        }
        private void ColorElement(IList<ElementId> elemIds, Autodesk.Revit.DB.Color color)
        {
            try
            {

                var ogs = new Autodesk.Revit.DB.OverrideGraphicSettings();

                var patternCollector = new FilteredElementCollector(_doc.ActiveView.Document);
                patternCollector.OfClass(typeof(Autodesk.Revit.DB.FillPatternElement));
                Autodesk.Revit.DB.FillPatternElement solidFill = patternCollector.ToElements().Cast<Autodesk.Revit.DB.FillPatternElement>().First(x => x.GetFillPattern().IsSolidFill);
#if REVIT2018
                ogs.SetProjectionFillColor(color);
                ogs.SetProjectionFillPatternId(solidFill.Id);
                ogs.SetProjectionLineColor(color);
                ogs.SetCutFillColor(color);
                ogs.SetCutFillPatternId(solidFill.Id);
                ogs.SetCutLineColor(color);
#else
                ogs.SetSurfaceForegroundPatternColor(color);
                ogs.SetSurfaceForegroundPatternId(solidFill.Id);
                ogs.SetProjectionLineColor(color);
                ogs.SetCutForegroundPatternColor(color);
                ogs.SetCutForegroundPatternId(solidFill.Id);
                ogs.SetCutLineColor(color);
#endif 

                using (Transaction t = new Transaction(_doc, "Set Element Override"))
                {
                    t.Start();
                    foreach (var id in elemIds)
                    {
                        _doc.ActiveView.SetElementOverrides(id, ogs);
                    }

                    t.Commit();
                }
            }
            catch
            {
                throw;
            }
        }


        private void RemoveColorEntireModel()
        {
            var view = _doc.ActiveView;
            var elems = ElementUtils.GetElementInstanceInProject(_doc);
            DisableTemperaryMode();
            removeColor(view, elems);
        }
        private void RemoveColorActiveView()
        {
            var view = _doc.ActiveView;
            var elems = ElementUtils.GetElementInstanceInView(_doc, view);
            removeColor(view, elems);
        }
        private void RemoveColorSelectedElement()
        {
            var view = _doc.ActiveView;
            var elems = ElementUtils.GetElementPreSelected(_doc);
            removeColor(view, elems);
        }

        private void removeColor(Autodesk.Revit.DB.View view, IList<Element> elems)
        {
            var elemIds = elems.Select(x => x.Id);
            try
            {
                OverrideGraphicSettings ogs = new OverrideGraphicSettings();

                using (Transaction t = new Transaction(_doc, "Set Element Override"))
                {
                    t.Start();
                    foreach (ElementId id in elemIds)
                    {
                        view.SetElementOverrides(id, ogs);
                    }
                    t.Commit();
                }
            }
            catch
            {
                throw;
            }
        }

        private void SaveSettingsJson()
        {
            JsonUtils.Save<ColorSettings>(TrackManagerViewModel.ColorSettingsFiles, TrackManagerViewModel._ColorSettings);
        }

        private static IList<TreeElementModel> Flatten(TreeElementModel root)
        {

            var flattened = new List<TreeElementModel> { root };

            var children = root.ChildrenNodes;

            if (children != null)
            {
                foreach (var child in children)
                {
                    flattened.AddRange(Flatten(child));
                }
            }
            return flattened;
        }
        #endregion //External events

    }
}