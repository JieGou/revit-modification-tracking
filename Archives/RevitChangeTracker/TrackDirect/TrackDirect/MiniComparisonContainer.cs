using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace TrackDirect.Models
{
    /// <summary>
    /// Contain information of project and list data container comparision
    /// </summary>
    public class MiniComparisonContainer
    {
        public static string ProjectId = "";
        public static Document Doc = null;
        public static Dictionary<string /*Unique Id element*/, MiniDataComparision /*Data comparison of element*/> PreviousData = new Dictionary<string, MiniDataComparision>();
        public static Dictionary<string /*Unique Id element*/, MiniDataComparision /*Data comparison of element*/> CurrentData = new Dictionary<string, MiniDataComparision>();

    }
   
}
