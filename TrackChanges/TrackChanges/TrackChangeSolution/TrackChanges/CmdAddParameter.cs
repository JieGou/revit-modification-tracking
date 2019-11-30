using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackChanges.TrackChangeSolution.TrackChanges
{
    [Transaction(TransactionMode.Manual]
    public class CmdAddParameter : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                CategorySet categories = CategoryUtils.GetCategoriesHasElements();
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
                throw;
            }
        }
    }
}
