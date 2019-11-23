using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace TrackChanges
{
    [Transaction(TransactionMode.Manual)]
    public class CmdShowFormWPF : IExternalCommand
    {
        public static UIApplication UIAPP { get; set; }
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIAPP = null;
            try
            {
                UIAPP = commandData.Application;
                
                AppTestWpf.Instance.ShowForm(commandData.Application);
                var wpf = AppTestWpf._wpfForm;
                MainViewModel vm = new MainViewModel();
                vm.ListView = GetAllView(UIAPP);
                vm.ElementIdList = GetElementIdList(UIAPP);
                wpf.DataContext = vm;
                return Result.Succeeded;
            }
            catch (Exception e)
            {

                message = e.Message;
                return Result.Failed;

            };
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

    }
}
