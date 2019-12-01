using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;

namespace TrackChanges
{
    public class WPFModel
    {
        private UIApplication _uiapp = null;
        private Application _app = null;
        private UIDocument _uidoc = null;
        private Document _doc = null;

        // The model constructor. Include a UIApplication argument and do all the assignments here.
        public WPFModel(UIApplication uiapp)
        {
            _uiapp = uiapp;
            _app = _uiapp.Application;
            _uidoc = _uiapp.ActiveUIDocument;
            _doc = _uidoc.Document;
        }

        // This function will be called by the Action function in the view model, so it must be public.
        public List<string> GenerateParametersAndValues(int idIntegerValue)
        {
            List<string> resstr = new List<string>();

            Element el = _doc.GetElement(new ElementId(idIntegerValue));
            if (el != null)
            {
                foreach (Parameter prm in el.Parameters)
                {
                    string str = prm.Definition.Name;
                    str += " : ";
                    str += prm.AsValueString();

                    resstr.Add(str);
                }
            }

            return resstr.OrderBy(x => x).ToList();
        }
    }
}
