using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace TrackChanges
{
    public class GetListElementsExEvent : IExternalEventHandler
    {
        public ExternalEvent 
        public void Execute(UIApplication app)
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            return "Get List Element";
        }
    }
}
